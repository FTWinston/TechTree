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
    var marginSize = 30, row = 0, maxY = 0, maxX = 0, num;
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

    var tree = $('techtree').css('width', (maxX - marginSize) + 'px').css('height', (maxY - marginSize - marginSize) + 'px');
    var branchHeight = marginSize / 2;

    $('techtree building[requires]').each(function (i, child) {
        var parentSymbol = child.getAttribute('requires');
        var parent = $('techtree building[symbol="' + parentSymbol + '"]')[0];

        var childX = child.offsetLeft + child.offsetWidth / 2;
        var childY = child.offsetTop;
        var parentX = parent.offsetLeft + parent.offsetWidth / 2;
        var parentY = parent.offsetTop + 3 * parent.offsetHeight / 4;

        if (childX == parentX) {
            tree.append(
                $('<link />')
                    .addClass('straight')
                    .css('top', parentY + 'px')
                    .css('left', parentX + 'px')
                    .css('height', (childY - parentY + child.offsetHeight / 4) + 'px')
            );
        }
        else {
            tree.append(
                $('<link />')
                    .addClass('straight')
                    .css('top', parentY + 'px')
                    .css('left', parentX + 'px')
                    .css('height', (childY - parentY - branchHeight) + 'px')
            );

            tree.append(
                $('<link />')
                    .addClass(childX < parentX ? 'left' : 'right')
                    .css('top', (childY - branchHeight) + 'px')
                    .css('left', Math.min(childX, parentX) + 'px')
                    .css('width', Math.abs(childX - parentX - (childX < parentX ? 2 : 0)) + 'px')
                    .css('height', (branchHeight + child.offsetHeight / 4) + 'px')
            );
        }
    });
}