
//timerId = setInterval("Button1_Click()", Interval);
function Button1_Click() {
    var progressDiv = document.getElementById('progressDiv')
    var div = document.createElement('div');

    div.style.display = 'block';
    div.style.cssFloat = 'left';
    div.style.styleFloat = 'left';
    div.style.width = '70px';
    div.style.height = '10px';
    div.style.backgroundColor = 'green';
    div.style.border = '0px solid black';

    progressDiv.appendChild(div);
    if (progressDiv.childNodes.length == 20)
        while (progressDiv.hasChildNodes())
            progressDiv.removeChild(progressDiv.firstChild);
}
var i = 0;
$(document).ready(function () {
    $("#reset").click(function () {
        $(':input', '#attachmentModal').val("");
        $("#pbarmain").hide();
        $("#pbar").hide();
        $(".progress-bar").css("width", "0%");
        i = 0;
    });
});
function makeProgress() {
    var Interval = $("#Cotime").val();
    var Sms = $("#SMS").val();
    var iNum = parseInt(Interval);
    console.log(Sms);
    console.log(Interval);
    $("#pbarmain").show();
    $("#pbar").show();

    if (i < 95) {
        i = i + 4;
        $(".progress-bar").css("width", i + "%").text(i + " %");
        setTimeout("makeProgress()", iNum);
    }
    else if (Sms != "" && Sms != null || Sms.Trim() != "undefined") {
       
        var result = confirm(Sms);
        if (result) {
            $("#pbarmain").hide();
            $("#pbar").hide();
        }
    }
}