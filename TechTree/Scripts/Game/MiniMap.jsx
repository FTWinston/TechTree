var MiniMap = React.createClass({
	componentDidMount: function() {
		var ctx = this.refs.canvas.getContext('2d');

		ctx.fillStyle = "rgb(200,0,0)";
		ctx.fillRect(10, 10, 22, 20);

		ctx.fillStyle = "rgba(0, 0, 200, 0.5)";
		ctx.fillRect(20, 20, 22, 20);
	},
	render: function() {
		return <canvas ref="canvas" width="100" height="80">Enable javascript to play</canvas>;
	}
});