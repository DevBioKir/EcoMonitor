enum District {
  Unknown,
  Kalininsky,
  Kurchatovsky,
  Leninsky,
  Metallurgichesky,
  Sovetsky,
  Traktorozavodsky,
  Central,
}

// Маппинг для отображения пользователю
const Map<District, String> districtNames = {
  District.Unknown: 'Неизвестно',
  District.Kalininsky: 'Калининский',
  District.Kurchatovsky: 'Курчатовский',
  District.Leninsky: 'Ленинский',
  District.Metallurgichesky: 'Металлургический',
  District.Sovetsky: 'Советский',
  District.Traktorozavodsky: 'Тракторозаводский',
  District.Central: 'Центральный',
};

District districtFromString(String? value) {
  if (value == null) return District.Unknown;
  return District.values.firstWhere(
    (d) => d.name.toLowerCase() == value.toLowerCase(),
    orElse: () => District.Unknown,
  );
}

String districtToString(District district) {
  return district.name;
}