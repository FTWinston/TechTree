var GameClient = React.createClass({
	getInitialState: function() {
		return { mainWidth: 800, mainHeight: 600 };
	},
	componentDidMount: function() {
		window.addEventListener('resize', this.handleResize.bind(this, false));
		this.handleResize();
	},
	componentWillUnmount: function() {
		window.removeEventListener('resize', this.handleResize);
	},
	handleResize: function(value, e) {
		this.setState({
			mainWidth: this.refs.self.offsetWidth - this.refs.side.offsetWidth,
			mainHeight: this.refs.self.offsetHeight - this.refs.bottom.offsetHeight,
		});
	},
    render: function() {
        return <div ref="self" className="game">
            <MainView ref="main" width={this.state.mainWidth} height={this.state.mainHeight} />
			<div ref="side" className="sideBar">
				<TreeViewer ref="tree" />
				<MiniMap ref="map" />
			</div>
			<div ref="bottom" className="bottomBar">
				buttons and stuff go here
			</div>
        </div>;
    }
});