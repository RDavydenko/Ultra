const getHashCode = (str: string) => {
  let hash = 0,
    i,
    chr;
  if (str.length === 0) return hash;
  for (i = 0; i < str.length; i++) {
    chr = str.charCodeAt(i);
    hash = (hash << 5) - hash + chr;
    hash |= 0; // Convert to 32bit integer
  }
  return hash;
};

export const getMarkerStyle = (sysName: string) => {
  const hashCode = getHashCode(sysName);
  const index = hashCode % COLOR_MARKER_STYLES.length;
  return COLOR_MARKER_STYLES[index];
};

export const COLOR_MARKER_STYLES = [
  {
    className: "hue-rotate-0",
    color: "#2880CA",
  },
  {
    className: "hue-rotate-45",
    color: "#7C65E5",
  },
  {
    className: "hue-rotate-90",
    color: "#CA50BD",
  },
  {
    className: "hue-rotate-135",
    color: "#E55068",
  },
  {
    className: "hue-rotate-180",
    color: "#BF681F",
  },
  {
    className: "hue-rotate-225",
    color: "#688000",
  },
  {
    className: "hue-rotate-270",
    color: "#1B9428",
  },
  {
    className: "hue-rotate-315",
    color: "#00947C",
  },
];
