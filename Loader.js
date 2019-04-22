$(".demo1").click(function () {
    $.showLoading({ allowHide: true });
});
$(".demo2").click(function () {
    $.showLoading({ name: 'jump-pulse', allowHide: true });
});
$(".demo3").click(function () {
    $.showLoading({ name: 'circle-turn', allowHide: true });
});
$(".demo4").click(function () {
    $.showLoading({ name: 'circle-turn-scale', allowHide: true });
});
$(".demo5").click(function () {
    $.showLoading({ name: 'circle-fade', allowHide: true });
});
$(".demo6").click(function () {
    $.showLoading({ name: 'square-flip', allowHide: true });
});
$(".demo7").click(function () {
    $.showLoading({ name: 'line-scale', allowHide: true });
});
$(".demo66").click(function () {
    if (confirm("Are you Sure???") == true)
    {
        $.showLoading({ name: 'circle-turn', allowHide: true });
    }
    else
    {
        return false;
    }

});

function myFun1() {

    $.showLoading({ name: 'square-flip', allowHide: true });

}