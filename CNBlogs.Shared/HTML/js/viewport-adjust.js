var pwidth, pheight;
var args = window.location.hash.split('&');
for (var i = 0; i < args.length; i++) {
    var pair = args[i].split('=');
    if (decodeURIComponent(pair[0]) == 'height') {
        pheight = Math.round(+decodeURIComponent(pair[1]));
    }
    else if (decodeURIComponent(pair[0]) == '#width') {
        pwidth = Math.round(+decodeURIComponent(pair[1]));
    }
}
document.write('<meta name="viewport" content="width=' + pwidth + ', height=' + pheight + ', , initial-scale=1.0">');
