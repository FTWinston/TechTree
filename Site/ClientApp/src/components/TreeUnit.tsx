import React, { FunctionComponent, useMemo, useContext } from 'react';
import { Icon, IconStyle } from './Icon';
import { TreeContext } from './TechTree';
import { EntityDetails } from './EntityDetails';

interface Props {
    id: number;
}

export const TreeUnit: FunctionComponent<Props> = props => {
    const treeContext = useContext(TreeContext);

    const unit = useMemo(
        () => treeContext.units[props.id],
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
                name={unit.name}
                style={iconStyle}
            >
                <EntityDetails entity={unit} />
            </Icon>
        </div>
    );
}