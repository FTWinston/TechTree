var TreeViewer = React.createClass({
	getInitialState: function() {
		return {expanded: false};
	},
	componentDidMount: function() {
		
	},
	render: function() {
		if (this.state.expanded)
		{
			var buildings = [];
			for (var i=0; i<this.props.tree.Buildings.length; i++) {
				var b = this.props.tree.Buildings[i];
				buildings.push(<TreeBuilding building={b} allUnits={this.props.tree.Units} key={i} />);
			}

			return <div className="treePopup" onClick={this.hideTree}>
				yo, this is the tree. It has {this.props.tree.Buildings.length} buildings & {this.props.tree.Units.length} units.
				<div className="techTree">
					{buildings}
				</div>
			</div>;
		}
		else
			return <div className="treeLink" onClick={this.showTree}>show tech tree</div>;
	},
	showTree: function() {
		this.setState({expanded: true});
	},
	hideTree: function() {
		this.setState({expanded: false});
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

			var builtAt = <div className="builtat" key={'b' + i}>{u.BuiltBy.Name}</div>;
            var requires = u.Prerequisite === undefined ? null : <div className="requires" key={'r' + i}>{u.Prerequisite.Name}</div>;
			units.push(<TreeStats entity={u} extra1={builtAt} extra2={requires} key={i} />);
		}

		return <div className="building" data-symbol={b.Symbol}>
			<TreeStats entity={b} extra1={upgrades} extra2={requires} />
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
			features.push(<TreeFeature feature={e.Features[i]} key={i} />);

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
		return <div className="feature"></div>;
	}
});