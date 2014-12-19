function setContent(content) {
    var contentDiv = document.getElementById("cnblogs_post_body");

    contentDiv.innerHTML = content;

    var codeDivs = document.getElementsByClassName("cnblogs_code_hide");

    // show codes
    if (codeDivs) {
        for (var i = 0; i < codeDivs.length; i++) {
            codeDivs[i].style.display = "block";
        }
    }

    // hide + and "View Code"
    var collapesImages = document.getElementsByClassName("code_img_closed");

    if (collapesImages) {
        for (var i = 0; i < collapesImages.length; i++) {
            collapesImages[i].style.display = "none";
        }
    }

    var titleDivs = document.getElementsByClassName("cnblogs_code_collapse");

    if (titleDivs) {
        for (var i = 0; i < titleDivs.length; i++) {
            titleDivs[i].style.display = "none";
        }
    }

    // wrap img
    $("#cnblogs_post_body img").each(function (index, element) {
        $(element).replaceWith("<div class='imgGrid'><div class='imgtable'><div class='imgtable-cell'>" +
            element.outerHTML + "</div></div></div>");
    });
}

//function removeImageFix()
//{
//    $("head link[rel='stylesheet']").last().remove();
//}