import React, { FunctionComponent } from 'react';
import { IEntityType } from '../../data/definitions/IEntityType';
import './EntityDetails.css';

interface Props {
    entity: IEntityType;
    prerequisite?: IEntityType;
}

export const EntityDetails: FunctionComponent<Props> = props => {
    const prerequisite = props.prerequisite
        ? <div className="entityDetails__prerequisite">Requires <span className="entityDetails__value">{props.prerequisite.symbol}</span></div>
        : undefined; // ought to use name here, but they're all the same currently

    return (
        <div className="entityDetails">
            <div className="entityDetails__name">{props.entity.name}</div>
            {prerequisite}
        </div>
    );
}