$(function () {
    $('building, unit').mouseover(function (e) {
        $(this).addClass('hover');
        e.stopPropagation();
    }).mouseout(function (e) {
        $(this).removeClass('hover');
        e.stopPropagation();
    });

    alignTree();
});

function alignTree() {
    var marginSize = 30, row = 0, maxY = marginSize, maxX = 0, num;
    do {
        var items = $('techtree building[row="' + row + '"]');
        items.css('top', maxY + 'px');

        var rowHeight = 0;

        items.each(function (i, e) {
            rowHeight = Math.max(rowHeight, e.offsetHeight);

            var left = (e.offsetWidth + marginSize) * e.getAttribute('column');

            $(e).css('left', left + 'px');
            maxX = Math.max(maxX, left + e.offsetWidth + marginSize);
        })

        // find the tallest item in this set, increase maxY by that plus some border amount
        maxY += rowHeight + marginSize;

        num = items.length;
        row++;
    } while (num > 0);

    $('techtree').css('width', maxX + 'px').css('height', maxY + 'px');
}