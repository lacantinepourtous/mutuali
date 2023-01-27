export async function getAddressFromGooglePlace(place) {
  let reverseGeocodeResult = await reverseGeocode(place.latitude, place.longitude);
  let address = { raw: JSON.stringify(place), latitude: place.latitude, longitude: place.longitude };

  if (place.locality !== undefined) {
    address.locality = { value: place.locality };
  }

  if (place.postal_code !== undefined) {
    address.postalCode = { value: place.postal_code };
  }

  if (place.route !== undefined) {
    address.route = { value: place.route };
  }

  if (place.street_number !== undefined) {
    address.streetNumber = { value: place.street_number };
  }

  if (reverseGeocodeResult.neighborhood !== undefined) {
    address.neighborhood = { value: reverseGeocodeResult.neighborhood };
  }

  if (reverseGeocodeResult.sublocality !== undefined) {
    address.sublocality = { value: reverseGeocodeResult.sublocality };
  }

  return address;
}

async function reverseGeocode(latitude, longitude) {
  const geocoder = new window.google.maps.Geocoder();
  const latlng = {
    lat: parseFloat(latitude),
    lng: parseFloat(longitude)
  };

  let neighborhood = "";
  let sublocality = "";

  let geocoded = await geocoder.geocode({ location: latlng });

  for (let i = 0; i < geocoded.results.length; i++) {
    let result = geocoded.results[i];

    if (neighborhood === "") {
      neighborhood = findComponent(result, "neighborhood");
    }
    if (sublocality === "") {
      sublocality = findComponent(result, "sublocality");
    }
  }

  return { neighborhood, sublocality };
}

function findComponent(result, type) {
  if (result.types.find((x) => x === type)) {
    let components = result.address_components;
    for (let j = 0; j < components.length; j++) {
      let component = components[j];
      if (component.types.find((x) => x === type)) {
        return component.long_name;
      }
    }
  }
  return "";
}
