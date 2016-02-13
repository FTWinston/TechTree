function processTree() {
    for (var i = 0; i < Tree.Buildings.length; i++) {
        var b = Tree.Buildings[i];
        b.BuildingNumber = i;
        
        if (b.Prerequisite !== undefined)
            b.Prerequisite = Tree.Buildings[b.Prerequisite];

        if (b.UpgradesFrom !== undefined)
            b.UpgradesFrom = Tree.Buildings[b.UpgradesFrom];

        for (var j = 0; j < b.Features.length; j++) {
            var f = b.Features[j];
            if (f.UnlockedBy !== undefined)
                f.UnlockedBy = Tree.Research[f.UnlockedBy];
        }
    }

    for (var i = 0; i < Tree.Units.length; i++) {
        var u = Tree.Units[i];
        u.UnitNumber = i;
        
        if (u.Prerequisite !== undefined)
            u.Prerequisite = Tree.Buildings[u.Prerequisite];

        if (u.BuiltBy !== undefined)
            u.BuiltBy = Tree.Buildings[u.BuiltBy];

        for (var j = 0; j < u.Features.length; j++) {
            var f = u.Features[j];
            if (f.UnlockedBy !== undefined)
                f.UnlockedBy = Tree.Research[f.UnlockedBy];
        }
    }

    for (var i = 0; i < Tree.Research.length; i++) {
        var r = Tree.Research[i];
        r.ResearchNumber = i;

        if (r.PerformedAt !== undefined)
            r.PerformedAt = Tree.Buildings[r.PerformedAt];
    }
}