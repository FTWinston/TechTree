import React, { FunctionComponent } from 'react';
import './Tooltip.css';

interface Props {
    className?: string;
}

export const Tooltip: FunctionComponent<Props> = props => {
    const className = props.className
        ? 'tooltip ' + props.className
        : 'tooltip';

    return (
        <div className={className}>
            <div className="tooltip__connector" />
            <div className="tooltip__content">
                {props.children}
            </div>
        </div>
    )
}