/*	ChillBox Packed - Version 1.2.1 - jQuery Plug-in
	by Christopher Hill - http://www.chillwebdesigns.co.uk/
	Last Modification: 07/01/10
	
	For more information, visit: http://www.chillwebdesigns.co.uk/chillbox.html
	
	Licensed under the Creative Commons Attribution 3.0 Unported License - http://creativecommons.org/licenses/by/3.0/
	- Free for use in both personal and commercial projects
	- Attribution requires leaving author name, author link, and the license info intact
*/  
	// Settings for modification.
	var OLC  =       "#000"; // Overlay color (hex)
	var OLO  =         "75"; // Overlay opacity (0-100%)
	var FIOL =          250; // Overlay fade in time (milliseconds)
	var LTC	 =       "#fff"; // Loading text color (hex)
	var LTS	 =         "12"; // Loading text size (px)
	var LT	 = "Loading..."; // Loading div text
	var CBBC =       "#000"; // ChillBox background color (hex)
	var CBTC =       "#fff"; // ChillBox title color (hex)
	var CBTS =         "12"; // ChillBox title text size 10-16 (px)
	var ECBC =		 "true"; // Enable ChillBox Group image counter
	var ST	 =       "true"; // ChillBox show all titles = true or hide all = false
	var BC   =       "#000"; // ChillBox button color (hex)
	var BTC	 =       "#fff"; // ChillBox button text color (hex)
	var BTS	 =         "11"; // ChillBox button text size 10-16 (px)
	var BBC	 =       "#333"; // ChillBox buttons border color (hex)
	var BBCH =    "#007eff"; // ChillBox buttons hover border color (hex)
	var CLSB =          "X"; // ChillBox close button text
	var PREV =          "<"; // ChillBox prev button text
	var NEXT =          ">"; // ChillBox next button text
	var CBFI =          500; // ChillBox fade in time (milliseconds)
	var CBFO =          500; // ChillBox fade out time (milliseconds)
	var EKBB =		 "true"; // Enable the use of keyboard buttons = true or disable =false
	// Keys for close are x, X, c, C & ESC and for Next N, n, > Key and for Prev P, p, < Key
	var CBCC =       "false"; // Enable close ChillBox by clicking the ChillBox image
	var OLCC =       "true"; // Enable close ChillBox by clicking the overlay
	var LOOP =       "false"; // Enabled loop = true or disable loop = false

// No conflict, run everything as jQuery, this allows prototype framework to be used with the jQuery framework.	
(function($) {	

	// Onload start ChillBox.
	$(function(){ 
	eval(function(p,a,c,k,e,d){e=function(c){return(c<a?'':e(parseInt(c/a)))+((c=c%a)>35?String.fromCharCode(c+29):c.toString(36))};if(!''.replace(/^/,String)){while(c--){d[e(c)]=k[c]||e(c)}k=[function(e){return d[e]}];e=function(){return'\\w+'};c=1};while(c--){if(k[c]){p=p.replace(new RegExp('\\b'+e(c)+'\\b','g'),k[c])}}return p}('L=$(2D);h=$(1A);8 23(2e){$(2e).2N(8(){$(\'<k/>\')[0].1n=r})}g 2c=$(\'a[c^=7]\').2A(8(i,e){M"\'"+$(e).G(\'1y\')+"\'"}).2j().34(\' , \');23(2c);$(\'[c=7]\').j(8(){g 16=$(r).G("1y");g 14=$(r).G("1U");$(\'<B 1k="7"><k 1n="\'+16+\'" /><a V="m">\'+1T+\'</a></B>\').H(\'1l\');b(28=="t"){$(\'<q>\'+14+\'</q>\').H(\'.7\')}7();M K});$(\'[c^=1F]\').j(8(T){g c=$(r).G(\'c\');g 1R=$(\'[c=\'+c+\']\').S();g T=$(\'[c=\'+c+\']\').T(r);$(r).G({\'7-u\':T});g 16=$(r).G("1y");g 14=$(r).G("1U");$(\'<B 1k="7"><k 1n="\'+16+\'" /><a V="m">\'+1T+\'</a></B>\').H(\'1l\');b(28=="t"){$(\'<q>\'+14+\'</q>\').H(\'.7\')}b($(\'[c=\'+c+\']\').S()>1){$(\'<a V="E">\'+2w+\'</a><a V="z">\'+2u+\'</a>\').H(\'.7\');b(2t=="t"){$(\'<q V="1S">\'+1E(T+1)+\' /   \'+1R+\'</q>\').H(\'.7\')}}b(25==\'t\'){$(\'.7 k\').j(8(){$(\'#m\').j()})}$(\'[c^=1F]\').1X(\'.Q\').2s(\'Q\');$(r).2x(\'Q\');g 1N=1E($(\'[c^=1F]\').1X(\'.Q\').G(\'7-u\'),10);b(2C=="K"){b($(\'[c=\'+c+\']\').2q().1W(\'Q\')){$(\'#E\').P()}b($(\'[c=\'+c+\']\').2z().1W(\'Q\')){$(\'#z\').P()}}b(2f=="t"){L.1K(8(e){b(e.v==37||e.v==2l){1b(8(){$(\'#z\').j()},1V)}});L.1K(8(e){b(e.v==2i||e.v==2h){1b(8(){$(\'#E\').j()},1V)}})}$(\'#z\').j(8(){$(\'.7\').P();1b(8(){g W=$(\'[c=\'+c+\']\').1c(1N-1);b(!W.S()){W=$(\'[c=\'+c+\']\').1c(0)}b(W.S()){W.j()}},1Y);M K});$(\'#E\').j(8(){$(\'.7\').P();1b(8(){g X=$(\'[c=\'+c+\']\').1c(1N+1);b(!X.S()){X=$(\'[c=\'+c+\']\').1c(0)}b(X.S()){X.j()}},1Y);M K});7();M K});8 7(){b($(\'.l\').36(\':2Z\')){g 1z="t"}b(1z!="t"||1z==2F){26(),D()};$(".7").d({C:\'0\',o:\'0\',1v:\'0\',p:\'0\',Y:\'0\',A:\'0\',f:\'0\',1M:\'30\',1P:\'1O\',17:1Z});$(".7 k").2Y(8(){b($().2X>="1.4.3"){g s=$(\'.7 k\').o();g n=$(\'.7 k\').f()}I{g s=13.d(13(\'.7 k\')[0],\'o\');g n=13.d(13(\'.7 k\')[0],\'f\')}b($.F.1j&&$.F.1i<=6||x.w==\'1C\'||x.w==\'1I\'||x.w==\'1H\'){h.2b(8(){$(".7").d({p:h.N()+h.f()/2+"y"})});$(".7").d({u:\'1f\',A:\'J.5%\',p:h.N()+h.f()/2+"y",f:(n+1)-n}).12({C:"1",18:-(s/2),1g:-(n/2)-15,o:(s+22),f:(n+1h)},19,8(){$(".7 k,.7 q,.7 a").U(19,\'1\')})}I{$(".7").d({u:\'1L\',p:\'J.5%\',A:\'J.5%\'}).12({C:"1",18:-(s/2),1g:-(n/2)-15,o:(s+22),f:(n+1h)},19,8(){$(".7 k,.7 q,.7 a").U(19,\'1\')})}$(".7 k").d({C:\'0\',11:\'32 1a 1d 1a\',R:\'0\'});$(".7 q").d({38:\'35\',C:\'0\',17:1Z,1u:33+"y",1J:2U,1x:\'A\',11:\'1e 1a\',R:\'1e 0\'});$(".7 q#1S").d({1x:\'Y\'});$(".7 a#m").d({11:\'Z 1a 21 1Q\',R:\'1e 1d\'});$(".7 a#m,.7 a#E,.7 a#z").d({17:2L,C:\'0\',1r:\'1B\',1J:2J,1u:2I+"y",1w:\'Z 1m\',1q:2a,1x:\'Y\',2G:\'2H\'});$(".7 a#E,.7 a#z").d({11:\'Z 1d 21 1Q\',R:\'1e 1d\'});$(".7 a#E,.7 a#z,.7 a#m").1p(8(){$(r).d({1w:\'Z 1m\',1q:2O})},8(){$(".7 a#E,.7 a#z,.7 a#m").d({1w:\'Z 1m\',1q:2a})});$(\'#m\').j(8(){$(".7 k,.7 q,.7 a,.D").P();$(\'.l\').U(1s,\'0\',8(){$(\'.l,.7\').P()});b($.F.1j&&$.F.1i<=6){$(".7").12({C:"0",A:\'J.5%\',p:$(1A).N()+$(1A).f()/2+"y",18:(s/2),1g:(n/2)-15,o:-(s+22),f:-(n+1h)},1s)}I{$(".7").12({C:"0",A:\'J.5%\',p:\'J.5%\',18:(s/2),1g:(n/2)-15,o:-(s+22),f:-(n+1h)},1s)}});b(2V==\'t\'){$(\'.l\').j(8(){$(\'#m\').j()});$(\'.l\').1p().d({1r:\'1B\'})}b(25==\'t\'){$(\'.7 k\').j(8(){$(\'#m\').j()});$(\'.7 k\').1p().d({1r:\'1B\'})}b(2f=="t"){L.2k(8(e){b(e.v==2p||e.v==2n){$(\'#m\').j()}});L.1K(8(e){b(e.v==27||e.v==2g||e.v==2E){$(\'#m\').j()}})}})}8 26(){$(\'<B 1k="l"></B>\').H(\'1l\');$(\'.l\').2y();1G();h.2d(8(){1G()});8 1G(){g 24=h.f();g 20=h.o();g 1D=L.f();$(\'.l\').d({17:2r,1v:\'0\',Y:\'0\',p:\'0\',A:\'0\',11:\'0\',R:\'0\',o:20,1M:\'2v\'});b(x.w==\'1C\'||x.w==\'1I\'||x.w==\'1H\'){$(\'.l\').d({u:\'1f\',f:1D})}I{b($.F.1j&&$.F.1i<=6){$(\'.l\').d({u:\'1f\',f:1D-$(\'.7\').f()})}I{$(\'.l\').d({u:\'1L\',f:24})}}}b(1E(O)<=\'9\'){$(\'.l\').U(1o,"0.0"+O)}I{b(O>=\'10\'&&O!=\'29\'){$(\'.l\').U(1o,"0."+O)}I{b(O==\'29\'){$(\'.l\').2B(1o)}}}}8 D(){$(\'1l\').2T(\'<B 1k="D">\'+2M+\'</B>\');$(\'.l,.7,.D\').2R("2Q",8(e){M K});1t();h.2d(8(){1t()});8 1t(){$(\'.D\').d({1P:\'1O\',o:\'2P\',1v:\'0\',p:h.f()/2,Y:\'0\',1u:2S+"y",2K:\'31\',A:h.o()/2-2W,1M:\'2o\',f:\'0\',1J:2m,u:\'1L\'});b($.F.1j&&$.F.1i<=6||x.w==\'1C\'||x.w==\'1I\'||x.w==\'1H\'){$(\'.D\').d({u:\'1f\',R:\'0\',p:h.N()+h.f()/2+"y"});$(".7").d({p:h.N()+h.f()/2+"y"});h.2b(8(){$(".D").d({p:h.N()+h.f()/2+"y"})})}}}',62,195,'|||||||ChillBox|function|||if|rel|css||height|var|win||click|img|Ov|close|imgHeight|width|top|span|this|imgWidth|true|position|keyCode|platform|navigator|px|prev|left|div|opacity|Loading|next|browser|attr|appendTo|else|49|false|doc|return|scrollTop|OLO|remove|selected|padding|size|index|fadeTo|id|to|too|right|1px||margin|animate|jQuery|tit||hf|backgroundColor|marginLeft|CBFI|11px|setTimeout|eq|5px|2px|absolute|marginTop|46|version|msie|class|body|solid|src|FIOL|hover|borderColor|cursor|CBFO|ReposLoad|fontSize|bottom|border|float|href|OLV|window|pointer|iPad|pageHeight|parseInt|ChillBoxGroup|ReposOv|iPod|iPhone|color|keydown|fixed|zIndex|current|Arial|fontFamily|4px|relSize|count|CLSB|title|250|hasClass|filter|500|CBBC|winWidth|0px||preload|winHeight|CBCC|Overlay||ST|100|BBC|scroll|arr|resize|arrayOfImages|EKBB|88|78|39|get|keypress|80|LTC|99|1002|120|last|OLC|removeClass|ECBC|PREV|1001|NEXT|addClass|hide|first|map|fadeIn|LOOP|document|67|null|textDecoration|none|BTS|BTC|textAlign|BC|LT|each|BBCH|100px|contextmenu|bind|LTS|append|CBTC|OLCC|50|jquery|load|visible|1003|center|12px|CBTS|join|normal|is||fontStyle'.split('|'),0,{}))

	// End of document ready function
	});
	
// End of no conflict function
})(jQuery) 