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
        var overallWidth = (this.props.map.maxX - this.props.map.minX) * this.props.cellRadius;
        var overallHeight = (this.props.map.maxY - this.props.map.minY) * this.props.cellRadius;

        return <div ref="outer" className="mainView" style={{width: this.props.width + 'px', height: this.props.height + 'px'}} onScroll={this.draw}>
            <canvas ref="canvas" width={this.props.width - this.props.scrollbarWidth} height={this.props.height - this.props.scrollbarHeight}>Enable javascript to play</canvas>
            <div className="scrollSize" style={{width: overallWidth + 'px', height: overallHeight + 'px'}} onClick={this.clicked} />
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
    _drawHex: function(ctx, cell, radius) {
        ctx.beginPath();
        
        var centerX = cell.xPos * radius + radius, centerY = cell.yPos * radius + radius;
        radius -= 0.4; // ensure there's always a 1px border drawn between cells

        var angle, x, y;
        for (var point = 0; point < 6; point++) {
            angle = 2 * Math.PI / 6 * (point + 0.5);
            x = centerX + radius * Math.cos(angle);
            y = centerY + radius * Math.sin(angle);

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
        
        if (cell.selected)
            ctx.fillStyle = '#fcc';

        ctx.fill();
    },
    clicked: function(e) {
        var cellIndex = this._getCellIndexAtPoint(e.clientX, e.clientY);

        var cell = this.props.map.Cells[cellIndex];
        if (cell === undefined || cell == null)
            return;

        if (cell.selected === true)
            cell.selected = undefined;
        else
            cell.selected = true;

        this.draw();
    },
    _getCellIndexAtPoint: function(screenX, screenY) {
        var mapX = screenX - this.refs.canvas.offsetLeft + this.refs.outer.scrollLeft + this.props.map.minX * this.props.cellRadius;
        var mapY = screenY - this.refs.canvas.offsetTop + this.refs.outer.scrollTop + this.props.map.minY * this.props.cellRadius;

        var fCol = (mapX * Math.sqrt(3) - mapY) / 3 / this.props.cellRadius;
        var fRow = mapY * 2 / 3 / this.props.cellRadius;
        var fThirdCoord = - fCol - fRow;

        var rCol = Math.round(fCol);
        var rRow = Math.round(fRow);
        var rThird = Math.round(fThirdCoord);

        var colDiff = Math.abs(rCol - fCol);
        var rowDiff = Math.abs(rRow - fRow);
        var thirdDiff = Math.abs(rThird - fThirdCoord);

        if (colDiff >= rowDiff) {
            if (colDiff >= thirdDiff)
                rCol = - rRow - rThird;
        }
        else if (rowDiff >= colDiff && rowDiff >= thirdDiff)
            rRow = - rCol - rThird;

        return rCol + rRow * this.props.map.Width;
    }
});