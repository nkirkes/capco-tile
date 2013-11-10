using System;
using System.Web;
using System.Net;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Configuration;
using com.mosso.cloudfiles;
using com.mosso.cloudfiles.domain;
using com.mosso.cloudfiles.exceptions;

namespace CAPCO.Infrastructure.Services
{
    public class CropInfo
    {
        public string Source { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int W { get; set; }
        public int H { get; set; }
    }

    public class CloudFilesService
    {
        private static string ValidImageFormats
        {
            get
            {
                return ConfigurationManager.AppSettings["ValidImageFormats"];
            }
        }

        private static string RackspaceUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["RackspaceUserName"];
            }
        }
        private string RackspaceAPIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["RackspaceAPIKey"];
            }
        }

        private string RackspaceProductImagesCDN
        {
            get
            {
                return ConfigurationManager.AppSettings["RackspaceProductImagesCDN"];
            }
        }

        private static string RackspaceProductImagesContainer
        {
            get
            {
                return ConfigurationManager.AppSettings["RackspaceProductImagesContainer"];
            }
        }

        private Connection CreateRackspaceConnection()
        {
            var userCredentials = new UserCredentials(RackspaceUserName, RackspaceAPIKey);
            return new Connection(userCredentials);
        }

        private string UniqueFileName(string extension)
        {
            string uid = Guid.NewGuid().ToString().Replace("-", "");
            return String.Format("{0}{1}", uid, extension);
        }

        /// <summary>
        /// Stores an image file on the Rackspace Cloud files CDN.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Http path to stored file</returns>
        public string UploadTempProfilePhoto(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0 && IsValidImageFile(file.FileName))
            {
                // set up the file name
                string uniqueFileName = UniqueFileName(Path.GetExtension(file.FileName));
                uniqueFileName = String.Format("{0}_to-crop{1}", Path.GetFileNameWithoutExtension(uniqueFileName), ".jpg");

                // resize the image to a generally workable size (this is "mostly" for the cropper UI so we don't have to deal with tiny pictures)
                Image originalImage = Image.FromStream(file.InputStream, true, true);
                Image imageToCrop = FileHelper.ResizeImageForCropping(originalImage, 450, 150, 450);

                // create the connection and image stream
                Connection connection = CreateRackspaceConnection();
                using (MemoryStream imgStream = new MemoryStream())
                {
                    // set up a higher quality jpg
                    ImageCodecInfo jpgInfo = ImageCodecInfo.GetImageEncoders().Where(codecInfo => codecInfo.MimeType == "image/jpeg").First();
                    using (EncoderParameters encParams = new EncoderParameters(1))
                    {
                        encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)100);
                        imageToCrop.Save(imgStream, jpgInfo, encParams);
                    }
                    // store the imageToCrop
                    imgStream.Seek(0, SeekOrigin.Begin);
                    connection.PutStorageItem(RackspaceProductImagesContainer, imgStream, uniqueFileName);
                }
                return String.Format("{0}/{1}", RackspaceProductImagesCDN, uniqueFileName);
            }
            else
            {
                throw new Exception("The file is not a valid image.");
            }
        }

        private void CreateAndStoreThumbnail(string newFileName, int width, int height, Image croppedImage)
        {
            // resize and save
            Connection connection = CreateRackspaceConnection();
            Image profileThumb = FileHelper.FixedSize(croppedImage, width, height);
            using (MemoryStream imgStream = new MemoryStream())
            {
                ImageCodecInfo jpgInfo = ImageCodecInfo.GetImageEncoders().Where(codecInfo => codecInfo.MimeType == "image/jpeg").First();
                using (EncoderParameters encParams = new EncoderParameters(1))
                {
                    encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)100);
                    profileThumb.Save(imgStream, jpgInfo, encParams);
                }
                //store it
                imgStream.Seek(0, SeekOrigin.Begin);
                connection.PutStorageItem(RackspaceProductImagesContainer, imgStream, newFileName);
            }
        }

        public bool Exists(string fileName)
        {
            Connection connection = CreateRackspaceConnection();
            try
            {
                var itemInfo = connection.GetStorageItemInformation(RackspaceProductImagesContainer, fileName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string CropAndSaveProfileImage(CropInfo cropInfo)
        {
            if (cropInfo == null)
                throw new ArgumentNullException("cropInfo", "cropInfo is null.");
            if (String.IsNullOrWhiteSpace(cropInfo.Source))
                throw new ArgumentNullException("cropInfo.Source", "Source file is null");

            // create filenames
            var fileName = Path.GetFileName(cropInfo.Source);
            try
            {
                string profileThumbFilename = String.Format("{0}_profile{1}", Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
                string smallThumbFilename = String.Format("{0}_small{1}", Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));

                // crop the original image
                var croppedImage = CropImage(FileHelper.GetImageFromUrl(cropInfo.Source), new Rectangle(cropInfo.X, cropInfo.Y, cropInfo.W, cropInfo.H));

                CreateAndStoreThumbnail(profileThumbFilename, 150, 150, croppedImage);
                CreateAndStoreThumbnail(smallThumbFilename, 75, 75, croppedImage);

            }
            catch (Exception ex)
            {
                throw;
            }
            return fileName;
        }

        private static Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpCrop = null;
            using (Bitmap bmpImage = new Bitmap(img))
            {
                bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            }
            return (Image)(bmpCrop);
        }

        private bool IsValidImageFile(string fileName)
        {
            return ValidImageFormats.IndexOf(Path.GetExtension(fileName).ToLower()) > -1;
        }
    }

    public static class FileHelper
    {
        public static Image GetImageFromUrl(string uri)
        {
            WebResponse result = null;
            Image image = null;

            try
            {
                WebRequest request = WebRequest.Create(uri);
                byte[] bytes;
                // Get the content
                result = request.GetResponse();
                Stream stream = result.GetResponseStream();

                // Bytes from address
                using (BinaryReader br = new BinaryReader(stream))
                {
                    // Ask for bytes bigger than the actual stream
                    bytes = br.ReadBytes(1000000);
                    br.Close();
                }
                // close down the web response object
                result.Close();

                // Bytes into image
                using (MemoryStream imageStream = new MemoryStream(bytes, 0, bytes.Length))
                {
                    imageStream.Write(bytes, 0, bytes.Length);
                    image = Image.FromStream(imageStream, true);
                    imageStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (result != null) result.Close();
            }

            return image;
        }

        //public static void SaveResizedImage(string filePath, string filename, string resizedFilename, int percent)
        //{
        //    Image origImg = Image.FromFile(Path.Combine(filePath, filename));
        //    Image resizedImg = ScaleByPercent(origImg, percent);

        //    resizedImg.Save(Path.Combine(filePath, resizedFilename), ImageFormat.Jpeg);
        //    resizedImg.Dispose();
        //    origImg.Dispose();
        //}

        //public static void SaveResizedImage(string filePath, string filename, string resizedFilename, int width, int height)
        //{
        //    Image origImg = Image.FromFile(Path.Combine(filePath, filename));
        //    Image resizedImg = FixedSize(origImg, width, height);

        //    resizedImg.Save(Path.Combine(filePath, resizedFilename), ImageFormat.Jpeg);
        //    resizedImg.Dispose();
        //    origImg.Dispose();
        //}

        public static Image ScaleByPercent(Image imgPhoto, int percent)
        {
            float nPercent = ((float)percent / 100);
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;
        }

        public static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            Bitmap result = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(imgPhoto, 0, 0, Width, Height);
            return (Image)result;
        }

        public static Image ResizeImageForCropping(Image img, int NewWidth, int minWidth, int MaxHeight)
        {
            // Prevent using images internal thumbnail
            img.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            img.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (img.Width <= NewWidth)
            {
                if (img.Width < minWidth)
                    NewWidth = minWidth;
                else
                    NewWidth = img.Width;
            }

            int NewHeight = img.Height * NewWidth / img.Width;
            if (NewHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = img.Width * MaxHeight / img.Height;
                NewHeight = MaxHeight;
            }

            return img.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);
        }

        /* 
         http://stackoverflow.com/questions/1171696/how-do-you-convert-a-httppostedfilebase-to-an-image
         try {
	        bitmap = new Bitmap(newWidth,newHeight);
	        using (Graphics g = Graphics.FromImage(bitmap))
	        {
		        g.SmoothingMode = SmoothingMode.HighQuality;
		        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
		        g.CompositingQuality = CompositingQuality.HighQuality;
		        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		        g.DrawImage(oldImage,
			        new Rectangle(0,0,newWidth,newHeight),
			        clipRectangle, GraphicsUnit.Pixel);
	        }//done with drawing on "g"
	        return bitmap;//IDisposable
        }
        catch
        {
	        if (bitmap != null) bitmap.Dispose();
	        throw;
        } 
         */

    }
}
