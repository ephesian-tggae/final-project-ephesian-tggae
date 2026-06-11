export function aggregateGenreCounts(itemsList) {
  const counts = {};

  for (const items of itemsList) {
    if (!items) {
      continue;
    }

    for (const item of items) {
      for (const genre of item.genres ?? []) {
        counts[genre.name] = (counts[genre.name] ?? 0) + 1;
      }
    }
  }

  return Object.entries(counts)
    .map(([name, count]) => ({ name, count }))
    .sort((a, b) => b.count - a.count || a.name.localeCompare(b.name));
}

export function computeAverageRating(reviews) {
  if (!reviews?.length) {
    return null;
  }

  const sum = reviews.reduce((total, review) => total + review.rating, 0);
  return Math.round((sum / reviews.length) * 10) / 10;
}
