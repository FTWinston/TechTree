import React, { FunctionComponent } from 'react';
import './EntityField.css';

interface Props {
    className?: string;
    name: string;
    value: number;
}

export const EntityField: FunctionComponent<Props> = props => {
    const classes = props.className
        ? `entityField ${props.className}`
        : 'entityField';

    return (
        <div className={classes}>
            <div className="entityField__label">{props.name}</div>
            <div className="entityField__value">{props.value}</div>
        </div>
    )
}