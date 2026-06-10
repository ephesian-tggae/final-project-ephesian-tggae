const messages = {
  watchlist: 'Your watchlist — saved to your MovieNest account.',
  history: 'Your watched history — saved to your MovieNest account.',
  reviews: 'Your ratings and reviews — saved to your MovieNest account.',
};

export default function UserDataNote({ context }) {
  return <p className="user-data-note">{messages[context]}</p>;
}
