var TreeViewer = React.createClass({
	getInitialState: function() {
		return {expanded: false};
	},
	render: function() {
		if (this.state.expanded)
		{
			return <div className="treePopup" onClick={this.hideTree}>yo, this is the tree</div>
		}
		else
			return <div className="treeLink" onClick={this.showTree}>tech tree ({this.props.tree.Buildings.length} / {this.props.tree.Units.length})</div>;
	},
	showTree: function() {
		this.setState({expanded: true});
	},
	hideTree: function() {
		this.setState({expanded: false});
	}
});