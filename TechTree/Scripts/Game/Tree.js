function processTree() {
    // first, set up links
    for (var i = 0; i < Tree.Buildings.length; i++) {
        var b = Tree.Buildings[i];
        
        if (b.Prerequisite !== undefined)
            b.Prerequisite = Tree.Buildings[b.Prerequisite];

        if (b.UpgradesFrom !== undefined)
            b.UpgradesFrom = Tree.Buildings[b.UpgradesFrom];
    }

    for (var i = 0; i < Tree.Units.length; i++) {
        var u = Tree.Units[i];
        
        if (u.Prerequisite !== undefined)
            u.Prerequisite = Tree.Buildings[u.Prerequisite];

        if (u.BuiltBy !== undefined)
            u.BuiltBy = Tree.Buildings[u.BuiltBy];
    }

    for (var i = 0; i < Tree.Research.length; i++) {
        var r = Tree.Research[i];

        if (r.PerformedAt !== undefined)
            r.PerformedAt = Tree.Buildings[r.PerformedAt];
    }
    /*
    // lastly, render links between buildings to show prerequisites
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
    */
}