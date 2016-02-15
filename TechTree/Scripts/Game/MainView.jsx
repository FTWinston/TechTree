var MainView = React.createClass({
    componentDidMount: function() {
        this.draw();
    },
    componentDidUpdate: function(prevProps, prevState) {
        this.draw();
    },
    render: function() {
        return <canvas ref="canvas" width={this.props.width} height={this.props.height}>Enable javascript to play</canvas>;
    },
    draw: function() {
        var ctx = this.refs.canvas.getContext('2d');
        ctx.clearRect(0, 0, this.props.width, this.props.height);

        var map = this.props.map, radius = this._determineCellRadiusToFit(this.props.width, this.props.height, map.Width, map.Height);
        var row, col, center;

        for (var i=0; i<map.Cells.length; i++) {
            if (map.Cells[i] == null)
                continue;
            center = this._getPixelCoordinates(i, radius);
            this._drawHex(ctx, center, radius);
        }
    },
    _determineCellRadiusToFit: function(widthPixels, heightPixels, mapWidth, mapHeight) {
		// add extra amounts for offset rows and bottom points of hexes on last row
        var packedHexWidth = widthPixels / (mapWidth + 0.5);
        var adjustedGridHeight = (heightPixels * this._packedHeightRatio) / (mapHeight + 0.5);
        var packedHexHeight =  adjustedGridHeight / this._packedHeightRatio;

        var ratio1 = packedHexWidth / this._packedWidthRatio, ratio2 = packedHexHeight / this._packedHeightRatio;
        return Math.min(ratio1, ratio2);
    },
    _packedWidthRatio: 1.7320508075688772,
    _packedHeightRatio: 1.5,
    _getPixelCoordinates: function(cellIndex, radius) {
        var row = Math.floor(cellIndex / map.Width);
        var col = cellIndex % map.Width;

		return {
		    x: radius * this._packedWidthRatio * (col + row/2) + radius,
		    y: radius * this._packedHeightRatio * row + radius
        };
    },
    _drawHex: function(ctx, center, radius) {
        ctx.beginPath();

        var angle, x, y;
        for (var point = 0; point < 6; point++) {
            angle = 2 * Math.PI / 6 * (point + 0.5);
            x = center.x + radius * Math.cos(angle);
            y = center.y + radius * Math.sin(angle);

            if (point === 0)
                ctx.moveTo(x, y);
            else
                ctx.lineTo(x, y);
        }

        ctx.fillStyle = '#888';
        ctx.fill();
    }
});