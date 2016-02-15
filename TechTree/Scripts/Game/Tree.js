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