const consDebug = true;

function __debug(msg) {
  if (consDebug) console.log(msg);
}

/**
 * Converts base64 string to hex
 * @param base64
 * @returns {string}
 */
const base64ToHex = function base64ToHex(base64) {
  const raw = window.atob(base64);

  let HEX = '';

  for (let i = 0; i < raw.length; i++) {
    const _hex = raw.charCodeAt(i).toString(16);

    HEX += _hex.length === 2 ? _hex : `0${_hex}`;
  }
  return HEX.toUpperCase();
};

/**
 * converts digest data to hash
 * @param  {object[]} dataIn is auth certificate
 * @param  {string} dataIn[].sessionId signature session Id
 * @param  {string} dataIn[].digest calculate digest in base64
 * @return {{
 *     type,
 *     hex
 * }}      returns hash data for signing
 */
const digestToHash = function digestToHash(dataIn) {
  const map = {};
  for (const digest of dataIn) {
    map[digest.sessionId] = base64ToHex(digest.digest);
  }
  __debug(`returned map ${JSON.stringify(map)}`);
  return {
    type: 'JSON',
    hex: map,
  };
};

/**
 * converts signature hex data
 * @param  {object[]} dataIn is auth certificate
 * @return {[{
 *     sessionId,
 *     signatureValue
 * }]}      returns session id with signature value in base64
 */
const hexToSignatureValues = function hexToSignatureValues(dataIn) {
  const result = [];
  Object.entries(dataIn).forEach((n) => {
    result.push({
      sessionId: n[0],
      signatureValue: hexToBase64(n[1]),
    });
  });
  return result;
};

/**
 * gets eid certificate
 * @param  {boolean} isAuth is auth certificate
 * @param  {string} lang language ui language
 * @return {Promise<{
 *     raw,
 *     base64
 * }>}      returns eid raw and base64 certificate
 */
const getEidCert = function getEidCert(isAuth, lang) {
  return new Promise((resolve, reject) =>
    eparakstshwcrypto
      .getCertificate({
        lang: lang || 'lv',
        operation: isAuth ? 'auth' : 'sign',
      })
      .then(
        (response) => {
          const cert = response;
          __debug(`Using certificate:\n${hexToPem(response.hex)}`);
          resolve({
            raw: cert,
            base64: hexToBase64(cert.hex),
          });
        },
        (err) => {
          __debug(`getCertificate() failed: ${err}`);
          reject(err);
        }
      )
  );
};

/**
 * signs hash with eparaksts extension
 * @param  {object} options options for signature
 * @param  {object[]} options.digests digests for signature
 * @param  {string} options.digests[].sessionId digest session id
 * @param  {string} options.digests[].digest digest data in base64
 * @param  {string} options.lang language ui language
 * @param  {boolean} options.isAuth is auth signature
 * @param  {object} options.cert raw cert
 * @return {Promise<[{
 *     sessionId,
 *     signatureValue
 * }]>}      Session Signature Value object
 */
const signEid = function signEid(options) {
  return new Promise((resolve, reject) => {
    const hash = digestToHash(options.digests);
    const lang = options.lang || 'lv';
    const operation = options.isAuth ? 'auth' : 'sign';
    eparakstshwcrypto.sign(options.cert, hash, { lang, operation }).then(
      (response) => {
        const signatureValues = hexToSignatureValues(response.hex);
        resolve(signatureValues);
      },
      (err) => {
        if (consDebug) console.error(`sign response ${JSON.stringify(err)}`);
        reject(err);
      }
    );
  });
};
