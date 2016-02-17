function setupGame(tree, map) {
    processTree(tree);
    processMap(map);

    var scrollbarSize = getScrollbarSize();
    
    var props = {
        tree: tree,
        map: map,
        scrollbarWidth: scrollbarSize.width,
        scrollbarHeight: scrollbarSize.height
    };

    ReactDOM.render(React.createElement(GameClient, props), document.getElementById('gameRoot'));

    function processTree(tree) {
        for (var i = 0; i < tree.Buildings.length; i++) {
            var b = tree.Buildings[i];
            b.BuildingNumber = i;

            if (b.Prerequisite !== undefined)
                b.Prerequisite = tree.Buildings[b.Prerequisite];

            if (b.UpgradesFrom !== undefined)
                b.UpgradesFrom = tree.Buildings[b.UpgradesFrom];

            for (var j = 0; j < b.Features.length; j++) {
                var f = b.Features[j];
                if (f.UnlockedBy !== undefined)
                    f.UnlockedBy = tree.Research[f.UnlockedBy];
            }
        }

        for (var i = 0; i < tree.Units.length; i++) {
            var u = tree.Units[i];
            u.UnitNumber = i;

            if (u.Prerequisite !== undefined)
                u.Prerequisite = tree.Buildings[u.Prerequisite];

            if (u.BuiltBy !== undefined)
                u.BuiltBy = tree.Buildings[u.BuiltBy];

            for (var j = 0; j < u.Features.length; j++) {
                var f = u.Features[j];
                if (f.UnlockedBy !== undefined)
                    f.UnlockedBy = tree.Research[f.UnlockedBy];
            }
        }

        for (var i = 0; i < tree.Research.length; i++) {
            var r = tree.Research[i];
            r.ResearchNumber = i;

            if (r.PerformedAt !== undefined)
                r.PerformedAt = tree.Buildings[r.PerformedAt];
        }
    }

    function processMap(map) {
        map.CellType = {
            OutOfBounds: 0,
            Flat: 1,
            Difficult: 2,
            Unpassable: 3,
            LowBarrier: 4,
            Barrier: 5,
        }

        var packedWidthRatio = 1.7320508075688772, packedHeightRatio = 1.5;
        var minX = Number.MAX_VALUE, minY = Number.MAX_VALUE;
        var maxX = Number.MIN_VALUE, maxY = Number.MIN_VALUE;

        for (var i = 0; i < map.Cells.length; i++) {
            var cell = map.Cells[i];
            if (cell == null)
                continue;

            cell.Row = Math.floor(i / map.Width);
            cell.Col = i % map.Width;
            cell.xPos = packedWidthRatio * (cell.Col + cell.Row/2);
            cell.yPos = packedHeightRatio * cell.Row;

            if (cell.xPos < minX)
                minX = cell.xPos;
            else if (cell.xPos > maxX)
                maxX = cell.xPos;

            if (cell.yPos < minY)
                minY = cell.yPos;
            else if (cell.yPos > maxY)
                maxY = cell.yPos;
        }

        map.minX = minX - 1; map.minY = minY - 1;
        map.maxX = maxX + 1; map.maxY = maxY + 1;

        for (var i = 0; i < map.Cells.length; i++) {
            var cell = map.Cells[i];
            if (cell == null)
                continue;

            cell.xPos -= minX;
            cell.yPos -= minY;
        }
    }

    function getScrollbarSize() {
        var outer = document.createElement("div");
        outer.style.visibility = "hidden";
        outer.style.width = "100px";
        outer.style.height = "100px";
        outer.style.msOverflowStyle = "scrollbar"; // needed for WinJS apps

        document.body.appendChild(outer);

        var widthNoScroll = outer.offsetWidth;
        var heightNoScroll = outer.offsetHeight;

        // force scrollbars
        outer.style.overflow = "scroll";

        // add innerdiv
        var inner = document.createElement("div");
        inner.style.width = "100%";
        inner.style.height = "100%";
        outer.appendChild(inner);

        var widthWithScroll = inner.offsetWidth;
        var heightWithScroll = inner.offsetHeight;

        // remove divs
        outer.parentNode.removeChild(outer);

        return {
            width: widthNoScroll - widthWithScroll,
            height: heightNoScroll - heightWithScroll
        }
    }
}