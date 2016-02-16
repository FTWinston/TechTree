var MainView = React.createClass({
    getDefaultProps: function() {
        return { cellRadius: 30 };
    },
    componentDidMount: function() {
        this.draw();
    },
    componentDidUpdate: function(prevProps, prevState) {
        this.draw();
    },
    render: function() {
        var overallWidth = this.props.cellRadius * 2 * (this.props.map.Width + 0.5);
        var overallHeight = this.props.cellRadius * 2 * (this.props.map.Height - 0.25);

        return <div ref="outer" className="mainView" style={{width: this.props.width + 'px', height: this.props.height + 'px'}} onScroll={this.draw}>
            <canvas ref="canvas" width={this.props.width - this.props.scrollbarWidth} height={this.props.height - this.props.scrollbarHeight}>Enable javascript to play</canvas>
            <div className="scrollSize" style={{width: overallWidth + 'px', height: overallHeight + 'px'}} />
        </div>
    },
    draw: function() {
        var ctx = this.refs.canvas.getContext('2d');
        ctx.clearRect(0, 0, this.props.width, this.props.height);
        ctx.translate(-this.refs.outer.scrollLeft, -this.refs.outer.scrollTop);

        var map = this.props.map;
        
        for (var i=0; i<map.Cells.length; i++) {
            if (map.Cells[i] == null)
                continue;
            var cell = map.Cells[i]
            this._drawHex(ctx, cell, this.props.cellRadius);
        }

        ctx.translate(this.refs.outer.scrollLeft, this.refs.outer.scrollTop);
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