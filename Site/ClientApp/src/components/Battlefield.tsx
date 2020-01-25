import React, { FunctionComponent, useState, useMemo, useRef, useLayoutEffect } from 'react';
import { IBattlefield } from '../data/instances/IBattlefield';
import { ICell } from '../data/instances/ICell';
import { ScrollCanvas } from './common/ScrollCanvas';
import './Battlefield.css';

interface Props {
    data: IBattlefield;
}

const defaultBounds = new DOMRect(0, 0, 1, 1);

export const Battlefield: FunctionComponent<Props> = props => {
    const canvas = useRef<HTMLCanvasElement>(null);
    
    const [bounds, setBounds] = useState<DOMRect>(defaultBounds);

    const [cellRadius, setCellRadius] = useState(30);
    
    const { cellPositions, minX, minY, maxX, maxY } = useMemo(
        () => positionCells(props.data),
        [props.data]
    );

    const { battlefieldWidth, battlefieldHeight } = useMemo(
        () => ({
            battlefieldWidth: (maxX - minX) * cellRadius,
            battlefieldHeight: (maxY - minY) * cellRadius,
        }),
        [cellRadius, minX, minY, maxX, maxY]
    )

    const context = useMemo(
        () => canvas.current
            ? canvas.current.getContext('2d')
            : null,
        [canvas.current]
    );

    const redraw = () => {
        if (!context) {
            return;
        }

        context.translate(-bounds.x, -bounds.y);
        context.clearRect(bounds.x, bounds.y, bounds.width, bounds.height);

        draw(cellPositions, context, bounds, cellRadius);
        context.translate(bounds.x, bounds.y);
    }

    useLayoutEffect(
        () => redraw(),
        [context, bounds, cellRadius]
    );

    const clicked = (x: number, y: number) => {
        const cellIndex = getCellIndexAtPoint(x, y);
        const cell = props.data.cells[cellIndex];

        if (!cell) {
            console.log(`clicked empty ${cellIndex} ${x}, ${y}`);
            return;
        }

        console.log(`clicked cell ${cellIndex} at ${x}, ${y}`);
        // cell.selected = !cell.selected;
        redraw();
    }

    return (
        <ScrollCanvas
            ref={canvas}
            className="battlefield"
            contentWidth={battlefieldWidth}
            contentHeight={battlefieldHeight}
            boundsChanged={setBounds}
            clicked={clicked}
        />
    );
}

interface Point {
    x: number;
    y: number;
}

function positionCells(battlefield: IBattlefield) {
    const packedWidthRatio = 1.7320508075688772, packedHeightRatio = 1.5;

    const cellPositions = new Map<ICell, Point>();
    let minX = Number.MAX_VALUE;
    let minY = Number.MAX_VALUE;
    let maxX = Number.MIN_VALUE;
    let maxY = Number.MIN_VALUE;

    for (let i = 0; i < battlefield.cells.length; i++) {
        const cell = battlefield.cells[i];
        if (cell === null) {
            continue;
        }

        const row = Math.floor(i / battlefield.width);
        const col = i % battlefield.width;
        const xPos = packedWidthRatio * (col + row / 2);
        const yPos = packedHeightRatio * row;
        
        if (xPos < minX) {
            minX = xPos;
        }
        else if (xPos > maxX) {
            maxX = xPos;
        }

        if (yPos < minY) {
            minY = yPos;
        }
        else if (yPos > maxY) {
            maxY = yPos;
        }

        cellPositions.set(cell, {x: xPos, y: yPos});
    }

    for (const [, val] of cellPositions) {
        val.x -= minX;
        val.y -= minY;
    }

    minX --;
    minY --;
    maxX ++;
    maxY ++;

    return {
        cellPositions,
        minX,
        minY,
        maxX,
        maxY,
    };
}

function draw(cellPositions: Map<ICell, Point>, ctx: CanvasRenderingContext2D, viewBounds: DOMRect, cellRadius: number) {

    for (const [cell, position] of cellPositions) {
        const dx = position.x * cellRadius + cellRadius;
        const dy = position.y * cellRadius + cellRadius;

        // TODO: check if cell is within viewBounds

        ctx.translate(dx, dy);
        drawHex(ctx, cell, cellRadius);
        ctx.translate(-dx, -dy);
    }
}

function drawHex(ctx: CanvasRenderingContext2D, cell: ICell, radius: number) {
    ctx.beginPath();
    
    radius -= 0.4; // ensure there's always a 1px border drawn between cells

    let angle, x, y;
    for (let point = 0; point < 6; point++) {
        angle = 2 * Math.PI / 6 * (point + 0.5);
        x = radius * Math.cos(angle);
        y = radius * Math.sin(angle);

        if (point === 0) {
            ctx.moveTo(x, y);
        } else {
            ctx.lineTo(x, y);
        }
    }

    switch (cell.type) {
        case 0: // 'OutOfBounds':
            ctx.fillStyle = '#000'; break;
        case 1: // 'Flat':
            ctx.fillStyle = '#ccc'; break;
        case 2: // 'Difficult':
            ctx.fillStyle = '#888'; break;
        case 3: // 'Unpassable':
            ctx.fillStyle = '#333'; break;
        default:
            ctx.fillStyle = '#a88'; break;
    }
    
    /*
    if (cell.selected)
        ctx.fillStyle = '#fcc';
    */

    ctx.fill();
}

function getCellIndexAtPoint(screenX: number, screenY: number) {
    /*
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
    */
    return 0;
}