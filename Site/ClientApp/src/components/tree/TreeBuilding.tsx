import React, { FunctionComponent, useMemo, useContext } from 'react';
import './TreeBuilding.css';
import { Icon, IconStyle } from '../common/Icon';
import { TreeLink } from './TreeLink';
import { TreeContext } from './TechTree';
import { TreeUnit } from './TreeUnit';
import { EntityDetails } from '../common/EntityDetails';

export enum Connection {
    None = 0,
    Middle = 1,
    Left = 2,
    Right = 4,
}

interface Props {
    id: number;
    incomingConnections: Connection;
    outgoingConnections: Connection;
}

export const TreeBuilding: FunctionComponent<Props> = props => {
    const treeContext = useContext(TreeContext);

    const building = useMemo(
        () => treeContext.buildings[props.id],
        [props.id, treeContext]
    );

    if (building === undefined) {
        return <div className="techTree__building" />;
    }

    const style: React.CSSProperties = useMemo(
        () => ({
            gridRow: building.displayRow,
            gridColumn: building.displayColumn,
        }),
        [building.displayRow, building.displayColumn]
    );

    const units = useMemo(
        () => {
            if (building.builds === undefined || building.builds.length === 0) {
                return undefined;
            }

            return building.builds.map(id => (
                <TreeUnit
                    key={id}
                    id={id}
                />
            ));
        },
        [building.builds, treeContext]
    );

    const builds = units === undefined
        ? undefined
        : <div className="treeBuilding__builds">{units}</div>

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

    let iconStyle = IconStyle.Normal
        
    if (false) {
        iconStyle |= IconStyle.Disabled;
    }

    return (
        <div className="techTree__building treeBuilding" style={style} data-id={props.id}>
            <Icon
                className="treeBuilding__icon techTree__icon"
                symbol={building.symbol}
                style={iconStyle}
            >
                <EntityDetails entity={building} />
            </Icon>
            {incoming}
            {outgoing}
            {builds}
        </div>
    );
}