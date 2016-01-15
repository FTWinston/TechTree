$(function () {
    $('.toggleTree').click(function () { $('#treeHolder').slideToggle(); });
    $('#treeHolder').hide();

    var canvas = $('#mainView')[0];
    if (!(canvas.getContext))
        return;

    var ctx = canvas.getContext('2d');

    ctx.fillStyle = "rgb(200,0,0)";
    ctx.fillRect(10, 10, 55, 50);

    ctx.fillStyle = "rgba(0, 0, 200, 0.5)";
    ctx.fillRect(30, 30, 55, 50);
});