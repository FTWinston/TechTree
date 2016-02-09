var MainView = React.createClass({
	componentDidMount: function() {
		var ctx = this.refs.canvas.getContext('2d');

		ctx.fillStyle = '#eedddd';
		ctx.fillRect(0, 0, this.props.width, this.props.height);

		ctx.fillStyle = "rgb(200,0,0)";
		ctx.fillRect(10, 10, 55, 50);

		ctx.fillStyle = "rgba(0, 0, 200, 0.5)";
		ctx.fillRect(30, 30, 55, 50);
	},
	render: function() {
		return <canvas ref="canvas" width={this.props.width} height={this.props.height}>Enable javascript to play</canvas>;
	}
});