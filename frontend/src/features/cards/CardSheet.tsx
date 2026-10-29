import { createPortal } from "react-dom";
import { CardFace, type CardFaceProps } from "./CardFace";

export type PrintableCard = CardFaceProps & { cardId: string };

function Pages({ cards }: { cards: PrintableCard[] }) {
  return Array.from({ length: Math.ceil(cards.length / 8) }, (_, page) => (
    <div className="card-page" key={page}>
      {cards.slice(page * 8, page * 8 + 8).map((card) => <CardFace key={card.cardId} {...card} />)}
    </div>
  ));
}

/** A body-level print portal hides the entire application shell, including dialog portals. */
export function CardSheet({ cards, screen = true }: { cards: PrintableCard[]; screen?: boolean }) {
  return <>
    {screen && <div className="card-screen-sheet"><Pages cards={cards} /></div>}
    {createPortal(<div className="card-print-root"><Pages cards={cards} /></div>, document.body)}
  </>;
}

