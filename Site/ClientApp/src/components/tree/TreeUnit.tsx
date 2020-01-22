import React, { FunctionComponent, useMemo, useContext } from 'react';
import { Icon, IconStyle } from '../common/Icon';
import { TreeContext } from './TechTree';
import { EntityDetails } from '../common/EntityDetails';

interface Props {
    id: number;
}

export const TreeUnit: FunctionComponent<Props> = props => {
    const treeContext = useContext(TreeContext);

    const [unit, prerequisite] = useMemo(
        () => {
            const unit = treeContext.units[props.id];
            const prerequisite = unit?.prerequisite
                ? treeContext.buildings[unit.prerequisite]
                : undefined;
            return [unit, prerequisite];
        },
        [props.id, treeContext]
    );

    if (unit === undefined) {
        return <div className="techTree__unit" />;
    }

    let iconStyle = !unit.prerequisite || unit.prerequisite === unit.builtBy
        ? IconStyle.Normal
        : IconStyle.Prerequisite

    if (false) {
        iconStyle |= IconStyle.Disabled;
    }

    return (
        <div className="techTree__unit treeUnit" data-id={props.id}>
            <Icon
                className="treeUnit__icon techTree__icon"
                symbol={unit.symbol}
                style={iconStyle}
            >
                <EntityDetails
                    entity={unit}
                    prerequisite={prerequisite}
                />
            </Icon>
        </div>
    );
}