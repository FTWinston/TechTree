$(function () {
    $('building, unit, feature').mouseover(function (e) {
        startHover(this);
        e.stopPropagation();
    }).mouseout(function (e) {
        var exited = this;
        timer = setTimeout(function () { stopHover(exited) }, 750);
        e.stopPropagation();
    });

    alignTree();
});

var lastHover = null;
var timer = null;

function startHover(element) {
    clearHover(element == lastHover ? null : lastHover);
    if (element == lastHover)
        return;

    $(element).addClass('hover');

    if (element.nodeName == 'FEATURE')
        $(element).closest('unit, building').addClass('hover');

    lastHover = element;
}

function stopHover(element) {
    if (lastHover != element)
        return;

    $(element).removeClass('hover');

    if (element.nodeName == 'FEATURE')
        $(element).closest('unit, building').removeClass('hover');

    lastHover = null;
    timer = null;
}

function clearHover(element) {
    if (timer != null) {
        clearTimeout(timer);
        timer = null;
    }
    if (element == null)
        return;

    $(lastHover).removeClass('hover');

    if (element.nodeName == 'FEATURE')
        $(element).closest('unit, building').removeClass('hover');
}

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