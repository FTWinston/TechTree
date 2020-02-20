import React, { useState, useEffect } from 'react';
import './App.css';
import { IGame } from './data/instances/IGame';
import { GameOverview } from './components/GameOverview';

const App: React.FC = () => {
    const [game, setGame] = useState<IGame>();

    useEffect(() => {
        loadGameDefinition(setGame);
    }, []);

    const gameDisplay = game
        ? <GameOverview data={game} />
        : undefined

    return (
        <div className="App">
            <button onClick={() => loadGameDefinition(setGame)}>load new game</button>
            {gameDisplay}
        </div>
    );
}

export default App;

async function loadGameDefinition(
    setGame: (data: IGame) => void
) {
    const response = await fetch('/Game/Generate');
    const data = await response.json() as IGame;
    console.log('received game', data);
    setGame(data);
}