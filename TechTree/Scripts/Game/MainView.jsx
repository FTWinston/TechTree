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
        
        for (var i=0; i<map.Cells.length; i++) {
            if (map.Cells[i] == null)
                continue;
            var cell = map.Cells[i]
            this._drawHex(ctx, cell, radius);
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
    _getPixelCoordinates: function(cell, radius) {
        return {
		    x: radius * this._packedWidthRatio * (cell.Col + cell.Row/2) + radius,
		    y: radius * this._packedHeightRatio * cell.Row + radius
        };
    },
    _drawHex: function(ctx, cell, radius) {
        var center = this._getPixelCoordinates(cell, radius);
        ctx.beginPath();
        radius -= 0.4; // ensure there's always a 1px border drawn between cells

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

        switch (cell.Type) {
            case map.CellType.OutOfBounds:
                ctx.fillStyle = '#000'; break;
            case map.CellType.Flat:
                ctx.fillStyle = '#ccc'; break;
            case map.CellType.Difficult:
                ctx.fillStyle = '#888'; break;
            case map.CellType.Unpassable:
                ctx.fillStyle = '#333'; break;
            default:
                ctx.fillStyle = '#a88'; break;
        }
        
        ctx.fill();
    }
});