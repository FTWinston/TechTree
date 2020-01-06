import React, { FunctionComponent, useMemo } from 'react';
import './TreeBuilding.css';
import { Icon } from './Icon';
import { TreeLink } from './TreeLink';

export enum Connection {
    None = 0,
    Middle = 1,
    Left = 2,
    Right = 4,
}

interface Props {
    id: number;
    symbol: string;
    name: string;
    row: number;
    column: number;
    incomingConnections: Connection;
    outgoingConnections: Connection;
}

export const TreeBuilding: FunctionComponent<Props> = props => {
    const style: React.CSSProperties = useMemo(
        () => ({
            gridColumnStart: props.column,
            gridColumnEnd: props.column + 1,
            gridRowStart: props.row,
            gridRowEnd: props.row + 1,
        }),
        [props.row, props.column]
    );

    const builds = props.children === undefined
        ? undefined
        : <div className="treeBuilding__builds">{props.children}</div>

    const incoming = props.incomingConnections === Connection.None
        ? undefined
        : <TreeLink
            className="treeBuilding__incoming"
            bottom={true}
            left={(props.incomingConnections & Connection.Left) !== 0}
            right={(props.incomingConnections & Connection.Right) !== 0}
            top={(props.incomingConnections & Connection.Middle) !== 0}
        />

    const outgoing = props.outgoingConnections === Connection.None
        ? undefined
        : <TreeLink
            className="treeBuilding__outgoing"
            top={true}
            left={false}
            right={false}
            bottom={false}
        />

    return (
        <div className="techTree__building treeBuilding" style={style} data-id={props.id}>
            <Icon
                className="treeBuilding__icon"
                symbol={props.symbol}
                name={props.name}
            />
            {incoming}
            {outgoing}
            {builds}
        </div>
    );
}