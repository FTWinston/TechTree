import React, { FunctionComponent, useMemo, useContext } from 'react';
import { IEntityType } from '../data/IEntityType';
import './EntityDetails.css';

interface Props {
    entity: IEntityType;
}

export const EntityDetails: FunctionComponent<Props> = props => {
    return (
        <div className="entityDetails">
            <div className="entityDetails__name">{props.entity.name}</div>
        </div>
    );
}