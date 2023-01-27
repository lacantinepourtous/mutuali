function randomPosition(lat, lng) {
  // OpenSource :: http://jsfiddle.net/siberex/f72ffn1x/
  let distance = 40;

  lat *= Math.PI / 180;
  lng *= Math.PI / 180;

  let radius;

  if (distance < 0) {
    radius = Math.abs(distance);
  } else {
    radius = Math.random() + Math.random();
    radius = radius > 1 ? 2 - radius : radius;
    radius *= distance ? distance : 10000;
  }

  radius /= 111319.9;
  radius *= Math.PI / 180;

  let angle = Math.random() * Math.PI * 2;

  let nLng,
    nLat = Math.asin(Math.sin(lat) * Math.cos(radius) + Math.cos(lat) * Math.sin(radius) * Math.cos(angle));
  if (Math.cos(nLat) === 0) {
    nLng = lng;
  } else {
    nLng = ((lng - Math.asin((Math.sin(angle) * Math.sin(radius)) / Math.cos(nLat)) + Math.PI) % (Math.PI * 2)) - Math.PI;
  }

  nLat *= 180 / Math.PI;
  nLng *= 180 / Math.PI;

  let pos = {};
  pos.lat = nLat;
  pos.lng = nLng;

  return pos;
}

export { randomPosition };
