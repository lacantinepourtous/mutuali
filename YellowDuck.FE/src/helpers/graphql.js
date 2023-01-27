async function addMaybeValue(inputSource, inputResult, name, handler) {
  if (name in inputSource) {
    let value = inputSource[name];

    if (handler) {
      value = await handler(value);
    }

    inputResult[name] = { value };
  }
}

export { addMaybeValue };
