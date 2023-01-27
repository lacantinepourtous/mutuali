// https://medium.com/commutatus/whats-going-on-with-the-heuristic-fragment-matcher-in-graphql-apollo-client-e721075e92be
// Run with: node ./schemaQuery.js

const fetch = require("node-fetch");
const fs = require("fs");

process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";

fetch(`https://localhost:44369/graphql`, {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    variables: {},
    query: `
      {
        __schema {
          types {
            kind
            name
            possibleTypes {
              name
            }
          }
        }
      }
    `
  })
})
  .then((result) => result.json())
  .then((result) => {
    // here we're filtering out any type information unrelated to unions or interfaces
    const filteredData = result.data.__schema.types.filter((type) => type.possibleTypes !== null);
    result.data.__schema.types = filteredData;
    fs.writeFileSync("./src/fragmentTypes.json", JSON.stringify(result.data), (err) => {
      if (err) {
        // eslint-disable-next-line no-console
        console.error("Error writing fragmentTypes file", err);
      } else {
        // eslint-disable-next-line no-console
        console.log("Fragment types successfully extracted!");
      }
    });
  });
