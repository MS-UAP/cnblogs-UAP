function setContent(content) {
    var contentDiv = document.getElementById("cnblogs_news_body");

    contentDiv.innerHTML = content;

    // remove custom style
    //$("span[style]:not(pre span), p[style]").each(function (index, element) {
    //    element.style.removeProperty('font-size');
    //});
}


function changeFontSize(size) {
    document.getElementsByTagName('html')[0].style.fontSize = size + 'em';

}
