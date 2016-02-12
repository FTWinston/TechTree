var TreeViewer = React.createClass({
	getInitialState: function() {
		return {expanded: false};
	},
	componentDidMount: function() {
		
	},
	render: function() {
		if (this.state.expanded)
		{
			var buildings = [], styles = new Array(this.props.tree.Buildings.length);

			var marginSize = 30, buildingWidth = 70, buildingHeight = 50, row = 0, maxY = 0, maxX = 0, numOnThisRow;
			do {
				var rowHeight = 0;
				numOnThisRow = 0;

				for (var i = 0; i < this.props.tree.Buildings.length; i++) {
					var b = this.props.tree.Buildings[i];
					if (b.DisplayRow != row)
						continue;
            
					numOnThisRow++;
					rowHeight = Math.max(rowHeight, buildingHeight * (this.countUnlockedUnits(b) + 1));
					var left = (buildingWidth + marginSize) * b.DisplayColumn;

					styles[i] = {top: maxY + 'px', left: left + 'px'};
					maxX = Math.max(maxX, left + buildingWidth + marginSize);
				}

				// find the tallest item in this set, increase maxY by that plus some border amount
				maxY += rowHeight + marginSize;
				row++;
			} while (numOnThisRow > 0);

			for (var i=0; i<this.props.tree.Buildings.length; i++) {
				var b = this.props.tree.Buildings[i];
				buildings.push(<TreeBuilding building={b} allUnits={this.props.tree.Units} key={i} onMouseOver={this.onMouseOver} onMouseOut={this.onMouseOut} style={styles[i]} />);
			}

		    var treeStyle = {width: (maxX - marginSize) + 'px', height: (maxY - marginSize - marginSize) + 'px'};

			return <div className="treePopup" onClick={this.hideTree}>
				yo, this is the tree. It has {this.props.tree.Buildings.length} buildings & {this.props.tree.Units.length} units.
				<div className="techTree" style={treeStyle}>
					{buildings}
				</div>
			</div>;
		}
		else
			return <div className="treeLink" onClick={this.showTree}>show tech tree</div>;
	},
    countUnlockedUnits: function(b) {
        var numChildren = 0;
        for (var i=0; i<this.props.tree.Units.length; i++)
            if (this.props.tree.Units[i].Prerequisite == b)
                numChildren++;
	    return numChildren;
    },
	showTree: function() {
		this.setState({expanded: true});
	},
	hideTree: function() {
		this.setState({expanded: false});
	},
	onMouseOver: function(e) {
		var element = e.currentTarget;

		this.clearHover(element == this.lastHover ? null : this.lastHover);
		if (element == this.lastHover)
			return;

		if (element.classList.contains('feature'))
			element.parentNode.classList.add('hover');

		element.classList.add('hover');
		this.lastHover = element;

		e.stopPropagation();
	},
	onMouseOut: function(e) {
		var self = this;
        this.mouseOutTimer = setTimeout(function () { self.delayMouseOut(e) }, 750);
		e.stopPropagation();
	},
	mouseOutTimer: null,
	lastHover: null,
	delayMouseOut: function(e) {
		var element = e.currentTarget;
		if (this.lastHover != element)
	        return;

		element.classList.remove('hover');
		
		if (element.classList.contains('feature'))
			element.parentNode.classList.remove('hover');

		this.lastHover = null;
		this.mouseOutTimer = null;
	},
	clearHover: function(element) {
		if (this.mouseOutTimer != null) {
			clearTimeout(this.mouseOutTimer);
			this.mouseOutTimer = null;
		}
		if (element == null)
			return;

		this.lastHover.classList.remove('hover');
		
		if (element.classList.contains('feature'))
			element.parentNode.classList.remove('hover');
	}
});

var TreeBuilding = React.createClass({
	render: function() {
		var b = this.props.building;

		var upgrades = b.UpgradesFrom === undefined ? null : <div className="upgradesfrom">{b.UpgradesFrom.Name}</div>;
		var requires = b.Prerequisite === undefined ? null : <div className="requires">{b.Prerequisite.Name}</div>;

		var units = [];
		for (var i=0; i<this.props.allUnits.length; i++) {
			var u = this.props.allUnits[i];
			if (u.Prerequisite != b)
				continue;

			var builtAt = <div className="builtat">{u.BuiltBy.Name}</div>;
            var requires = u.Prerequisite === undefined ? null : <div className="requires">{u.Prerequisite.Name}</div>;
            units.push(<div className="unit" data-symbol={u.Symbol} key={i} onMouseOver={this.props.onMouseOver} onMouseOut={this.props.onMouseOut}>
				<TreeStats entity={u} extra1={builtAt} extra2={requires} onMouseOver={this.onMouseOver} onMouseOut={this.onMouseOut} />
			</div>);
		}

		return <div className="building" data-symbol={b.Symbol} onMouseOver={this.props.onMouseOver} onMouseOut={this.props.onMouseOut} style={this.props.style}>
			<TreeStats entity={b} extra1={upgrades} extra2={requires} onMouseOver={this.onMouseOver} onMouseOut={this.onMouseOut} />
			<div className="unlocks">
				{units}
			</div>
		</div>;
	}
});

var TreeStats = React.createClass({
	render: function() {
		var e = this.props.entity;

		var minerals = e.MineralCost == 0 ? null : <div className="minerals">{e.MineralCost}</div>;
		var vespine = e.VespineCost == 0 ? null : <div className="vespine">{e.VespineCost}</div>;
		var supply = e.SupplyCost == 0 ? null : <div className={e.SupplyCost < 0 ? "supply positive" : "supply"}>{Math.abs(e.SupplyCost)}</div>;

		var mana = e.Mana == 0 ? null : <div className="mana">{e.Mana}</div>;
		var action = e.ActionPoints == 0 ? null : <div className="actionpoints">{e.ActionPoints}</div>;

		var detector = e.IsDetector ? <div className="detector" /> : null;

		var features = [];
		for (var i=0; i<e.Features.length; i++)
			features.push(<TreeFeature feature={e.Features[i]} key={i} onMouseOver={this.onMouseOver} onMouseOut={this.onMouseOut} />);

		return <div className="info">
			<div className="name">{e.Name}</div>
			<div className="cost">
				{minerals}
				{vespine}
				{supply}
				<div className="time">{e.BuildTime}</div>
			</div>
			<div className="stats">
				<div className="health">{e.Health}</div>
				<div className="armor">{e.Armor}</div>
				{mana}
				{action}
				<div className="vision">{e.VisionRange}</div>
			</div>
			{detector}
			{this.props.extra1}
            {this.props.extra2}
			{features}
		</div>
	}
});

var TreeFeature = React.createClass({
	render: function() {
		var f = this.props.feature;

		var unlocked = f.UnlockedBy === undefined ? null : <div className="unlocked">f.UnlockedBy.PerformedAt.Name</div>;

		var stats = null, cost = null;
		if (f.UsesMana || f.LimitedUses > 0 || f.CooldownTurns > 0) {
			var mana = f.UsesMana ? <div className="mana">{f.ActivateManaCost > 0 || f.ManaCostPerTurn > 0 ? (f.ActivateManaCost > 0 ? f.ActivateManaCost : 0) + " to activate, " + (f.ManaCostPerTurn > 0 ? f.ManaCostPerTurn : 0) + " per turn" : f.ManaCost}</div> : null;
			var uses = f.LimitedUses > 0 ? <div className="uses">{f.LimitedUses}</div> : null;
			var cooldown = f.CooldownTurns > 0 ? <div className="cooldown">{f.CooldownTurns}</div> : null;

			stats = <div className="stats">
				{mana}
				{uses}
				{cooldown}
			</div>;
		}

		if (f.MineralCost > 0 || f.VespineCost > 0 || f.SupplyCost > 0 || f.SupplyCost < 0 || f.BuildTime > 0) {
			var minerals = f.MineralCost == 0 ? null : <div className="minerals">{f.MineralCost}</div>;
			var vespine = f.VespineCost == 0 ? null : <div className="vespine">{f.VespineCost}</div>;
			var supply = f.SupplyCost == 0 ? null : <div className={f.SupplyCost < 0 ? "supply positive" : "supply"}>{Math.abs(f.SupplyCost)}</div>;
			var time = f.BuildTime == 0 ? null : <div className="time">{f.BuildTime}</div>;
			cost = <div className="cost">
				{minerals}
				{vespine}
				{supply}
				{time}
			</div>;
		}

		return <div className="feature" data-symbol={f.Symbol} data-mode={f.Mode} onMouseOver={this.props.onMouseOver} onMouseOut={this.props.onMouseOut}>
			<div className="info">
				<div className="name">{f.Name}</div>
				{stats}
				{cost}
				{unlocked}
				<div className="description">f.Description</div>
			</div>
		</div>;
	}
});