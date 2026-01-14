/*! This is eparaksts-hwcrypto.js 1.2.0 generated on 2018-09-24 */
/* DO NOT EDIT (use src/eparaksts-hwcrypto.js) */
const eparakstshwcrypto = (function eparakstshwcrypto() {
  let consoleDebug = false;
  const _debug = function (x) {
    if (consoleDebug) console.log(x);
  };
  _debug('eparaksts-hwcrypto.js activated');
  window.addEventListener = window.addEventListener || window.attachEvent;
  function hasPluginFor(mime) {
    return navigator.mimeTypes && mime in navigator.mimeTypes;
  }
  function hasExtension(cls) {
    return typeof window[cls] === 'function';
  }
  function _hex2array(str) {
    if (typeof str === 'string') {
      const len = Math.floor(str.length / 2);
      const ret = new Uint8Array(len);
      for (let i = 0; i < len; i++) {
        ret[i] = parseInt(str.substr(i * 2, 2), 16);
      }
      return ret;
    }
  }
  function _array2hex(args) {
    let ret = '';
    for (let i = 0; i < args.length; i++) ret += (args[i] < 16 ? '0' : '') + args[i].toString(16);
    return ret.toLowerCase();
  }
  function _mimeid(mime) {
    return `hwc${mime.replace('/', '').replace('-', '')}`;
  }
  function loadPluginFor(mime) {
    const element = _mimeid(mime);
    if (document.getElementById(element)) {
      _debug('Plugin element already loaded');
      return document.getElementById(element);
    }
    _debug(`Loading plugin for ${mime} into ${element}`);
    const objectTag = `<object id="${element}" type="${mime}" style="width: 1px; height: 1px; position: absolute; visibility: hidden;"></object>`;
    const div = document.createElement('div');
    div.setAttribute('id', `pluginLocation${element}`);
    document.body.appendChild(div);
    document.getElementById(`pluginLocation${element}`).innerHTML = objectTag;
    return document.getElementById(element);
  }
  const signing_plugin_mime = 'application/x-eparaksts';
  const signing_extension = 'eParakstsTokenSigning';
  const USER_CANCEL = 'user_cancel';
  const NO_CERTIFICATES = 'no_certificates';
  const INVALID_ARGUMENT = 'invalid_argument';
  const TECHNICAL_ERROR = 'technical_error';
  const NO_IMPLEMENTATION = 'no_implementation';
  const NOT_ALLOWED = 'not_allowed';
  function probe() {
    const msg = 'probe() detected ';
    if (hasExtension(signing_extension)) {
      _debug(msg + signing_extension);
    }
    if (hasPluginFor(signing_plugin_mime)) {
      _debug(msg + signing_plugin_mime);
    }
  }
  window.addEventListener('load', (event) => {
    probe();
  });
  function SigningPlugin() {
    this._name = `NPAPI/BHO for ${signing_plugin_mime}`;
    const p = loadPluginFor(signing_plugin_mime);
    const certificate_ids = {};
    function code2str(err) {
      _debug(`Error: ${err} with: ${p.errorMessage}`);
      switch (parseInt(err)) {
        case 1:
          return USER_CANCEL;

        case 2:
          return NO_CERTIFICATES;

        case 17:
          return INVALID_ARGUMENT;

        case 19:
          return NOT_ALLOWED;

        default:
          _debug(`Unknown error: ${err} with: ${p.errorMessage}`);
          return TECHNICAL_ERROR;
      }
    }
    function code2err(err) {
      return new Error(code2str(err));
    }
    this.check = function () {
      return new Promise((resolve, reject) => {
        setTimeout(() => {
          resolve(typeof p.version !== 'undefined');
        }, 0);
      });
    };
    this.getVersion = function () {
      return new Promise((resolve, reject) => {
        const v = p.version;
        resolve(v);
      });
    };
    this.getCertificate = function (options) {
      const operation = options.operation || 'sign';
      const language = options.lang || 'en';
      p.pluginLanguage = language;
      return new Promise((resolve, reject) => {
        try {
          let v;
          if (typeof p.getCertificateWithOperation !== 'undefined') {
            v = p.getCertificateWithOperation(operation);
          } else {
            v = p.getCertificate();
          }
          if (parseInt(p.errorCode) !== 0) {
            reject(code2err(p.errorCode));
          } else {
            certificate_ids[v.cert] = v.id;
            resolve({
              hex: v.cert,
            });
          }
        } catch (ex) {
          _debug(ex);
          reject(code2err(p.errorCode));
        }
      });
    };
    this.sign = function (cert, hash, options) {
      return new Promise((resolve, reject) => {
        const cid = certificate_ids[cert.hex];
        if (cid) {
          try {
            const language = options.lang || 'en';
            const operation = options.operation || 'sign';
            let v;
            if (typeof p.signWithOperation !== 'undefined') {
              v = p.signWithOperation(cid, hash.hex, language, operation);
            } else {
              v = p.sign(cid, hash.hex, language);
            }
            resolve({
              hex: v,
            });
          } catch (ex) {
            _debug(JSON.stringify(ex));
            reject(code2err(p.errorCode));
          }
        } else {
          _debug(`invalid certificate: ${cert}`);
          reject(new Error(INVALID_ARGUMENT));
        }
      });
    };
  }
  function SigningExtension() {
    this._name = 'Native Messaging extension';
    let p = null;
    this.check = function () {
      return new Promise((resolve, reject) => {
        if (!hasExtension(signing_extension)) {
          return resolve(false);
        }
        p = new window[signing_extension]();
        if (p) {
          resolve(true);
        } else {
          resolve(false);
        }
      });
    };
    this.getVersion = function () {
      return p.getVersion();
    };
    this.getCertificate = function (options) {
      return p.getCertificate(options);
    };
    this.sign = function (cert, hash, options) {
      return p.sign(cert, hash, options);
    };
  }
  function NoBackend() {
    this._name = 'No implementation';
    this.check = function () {
      return new Promise((resolve, reject) => {
        resolve(true);
      });
    };
    this.getVersion = function () {
      return Promise.reject(new Error(NO_IMPLEMENTATION));
    };
    this.getCertificate = function () {
      return Promise.reject(new Error(NO_IMPLEMENTATION));
    };
    this.sign = function () {
      return Promise.reject(new Error(NO_IMPLEMENTATION));
    };
  }
  let _backend = null;
  const fields = {};
  function _testAndUse(Backend) {
    return new Promise((resolve, reject) => {
      const b = new Backend();
      b.check().then((isLoaded) => {
        if (isLoaded) {
          _debug(`Using backend: ${b._name}`);
          _backend = b;
          resolve(true);
        } else {
          _debug(`${b._name} check() failed`);
          resolve(false);
        }
      });
    });
  }
  function _autodetect(force) {
    return new Promise((resolve, reject) => {
      _debug('Autodetecting best backend');
      if (typeof force === 'undefined') {
        force = false;
      }
      if (_backend !== null && !force) {
        return resolve(true);
      }
      function trySigningPlugin() {
        _testAndUse(SigningPlugin).then((result) => {
          if (result) {
            resolve(true);
          } else {
            resolve(_testAndUse(NoBackend));
          }
        });
      }
      if (
        navigator.userAgent.indexOf('MSIE') != -1 ||
        navigator.userAgent.indexOf('Trident') != -1
      ) {
        _debug('Assuming IE BHO, testing');
        return trySigningPlugin();
      }
      if (hasExtension(signing_extension)) {
        _testAndUse(SigningExtension).then((result) => {
          if (result) {
            resolve(true);
          } else {
            trySigningPlugin();
          }
        });
        return;
      }
      if (hasPluginFor(signing_plugin_mime)) {
        return trySigningPlugin();
      }
      resolve(_testAndUse(NoBackend));
    });
  }
  fields.use = function (backend) {
    return new Promise((resolve, reject) => {
      if (typeof backend === 'undefined' || backend === 'auto') {
        _autodetect().then((result) => {
          resolve(result);
        });
      } else if (backend === 'chrome') {
        resolve(_testAndUse(SigningExtension));
      } else if (backend === 'npapi') {
        resolve(_testAndUse(SigningPlugin));
      } else {
        resolve(false);
      }
    });
  };
  fields.getBackendInfo = function () {
    return new Promise((resolve, reject) => {
      _autodetect().then((result) => {
        _backend.getVersion().then(
          (version) => {
            resolve({
              type: _backend._name,
              version,
            });
          },
          (error) => {
            if (hasExtension(signing_extension)) {
              resolve({
                type: 'FAILING_NATIVE_MESSAGING',
              });
            } else if (hasPluginFor(signing_plugin_mime)) {
              resolve({
                type: 'FAILING_NPAPI',
              });
            } else {
              resolve({
                type: 'NO_IMPLEMENTATION',
              });
            }
          }
        );
      });
    });
  };
  fields.getBackendType = function () {
    return new Promise((resolve, reject) => {
      _autodetect().then((result) => {
        _backend.getVersion().then(
          (version) => {
            resolve(_backend._name);
          },
          (error) => {
            if (hasExtension(signing_extension)) {
              resolve('FAILING_NATIVE_MESSAGING');
            } else if (hasPluginFor(signing_plugin_mime)) {
              resolve('FAILING_NPAPI');
            } else {
              resolve('NO_IMPLEMENTATION');
            }
          }
        );
      });
    });
  };
  fields.setConsoleDebug = function (x) {
    consoleDebug = x;
  };
  fields.debug = function () {
    return new Promise((resolve, reject) => {
      const hwversion = 'eparakstshwcrypto 1.2.0';
      _autodetect().then((result) => {
        _backend.getVersion().then(
          (version) => {
            resolve(`${hwversion} with ${_backend._name} ${version}`);
          },
          (error) => {
            resolve(`${hwversion} with failing backend ${_backend._name}`);
          }
        );
      });
    });
  };
  fields.getCertificate = function (options) {
    if (typeof options !== 'object') {
      _debug('getCertificate options parameter must be an object');
      return Promise.reject(new Error(INVALID_ARGUMENT));
    }
    if (options && !options.operation) {
      options.operation = 'sign';
    }
    if (options && !options.lang) {
      options.lang = 'en';
    }
    return _autodetect().then((result) => {
      if (location.protocol !== 'https:' && location.protocol !== 'file:') {
        return Promise.reject(new Error(NOT_ALLOWED));
      }
      return _backend.getCertificate(options).then((certificate) => {
        if (certificate.hex && !certificate.encoded)
          certificate.encoded = _hex2array(certificate.hex);
        return certificate;
      });
    });
  };
  fields.sign = function (cert, hash, options) {
    if (arguments.length < 2) return Promise.reject(new Error(INVALID_ARGUMENT));
    if (options && !options.operation) {
      options.operation = 'sign';
    }
    if (options && !options.lang) {
      options.lang = 'en';
    }
    if (!hash.type || (!hash.value && !hash.hex))
      return Promise.reject(new Error(INVALID_ARGUMENT));
    if (hash.hex && !hash.value) {
      _debug('DEPRECATED: hash.hex as argument to sign() is deprecated, use hash.value instead');
      hash.value = _hex2array(hash.hex);
    }
    if (hash.value && !hash.hex) hash.hex = _array2hex(hash.value);
    return _autodetect().then((result) => {
      if (location.protocol !== 'https:' && location.protocol !== 'file:') {
        return Promise.reject(new Error(NOT_ALLOWED));
      }
      return _backend.sign(cert, hash, options).then((signature) => {
        if (signature.hex && !signature.value) signature.value = _hex2array(signature.hex);
        return signature;
      });
    });
  };
  fields.NO_IMPLEMENTATION = NO_IMPLEMENTATION;
  fields.USER_CANCEL = USER_CANCEL;
  fields.NOT_ALLOWED = NOT_ALLOWED;
  fields.NO_CERTIFICATES = NO_CERTIFICATES;
  fields.TECHNICAL_ERROR = TECHNICAL_ERROR;
  fields.INVALID_ARGUMENT = INVALID_ARGUMENT;
  return fields;
})(); // FIXME: origin unknown
if (!window.atob) {
  const tableStr = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
  const table = tableStr.split('');

  window.atob = function (base64) {
    if (/(=[^=]+|={3,})$/.test(base64)) throw new Error('String contains an invalid character');
    base64 = base64.replace(/=/g, '');
    const n = base64.length & 3;
    if (n === 1) throw new Error('String contains an invalid character');
    for (var i = 0, j = 0, len = base64.length / 4, bin = []; i < len; ++i) {
      const a = tableStr.indexOf(base64[j++] || 'A');
      const b = tableStr.indexOf(base64[j++] || 'A');
      const c = tableStr.indexOf(base64[j++] || 'A');
      const d = tableStr.indexOf(base64[j++] || 'A');
      if ((a | b | c | d) < 0) throw new Error('String contains an invalid character');
      bin[bin.length] = ((a << 2) | (b >> 4)) & 255;
      bin[bin.length] = ((b << 4) | (c >> 2)) & 255;
      bin[bin.length] = ((c << 6) | d) & 255;
    }
    return String.fromCharCode.apply(null, bin).substr(0, bin.length + n - 4);
  };

  window.btoa = function (bin) {
    for (var i = 0, j = 0, len = bin.length / 3, base64 = []; i < len; ++i) {
      const a = bin.charCodeAt(j++);
      const b = bin.charCodeAt(j++);
      const c = bin.charCodeAt(j++);
      if ((a | b | c) > 255) throw new Error('String contains an invalid character');
      base64[base64.length] =
        table[a >> 2] +
        table[((a << 4) & 63) | (b >> 4)] +
        (isNaN(b) ? '=' : table[((b << 2) & 63) | (c >> 6)]) +
        (isNaN(b + c) ? '=' : table[c & 63]);
    }
    return base64.join('');
  };
}

function hexToBase64(str) {
  return btoa(
    String.fromCharCode.apply(
      null,
      str
        .replace(/\r|\n/g, '')
        .replace(/([\da-fA-F]{2}) ?/g, '0x$1 ')
        .replace(/ +$/, '')
        .split(' ')
    )
  );
}

function hexToPem(s) {
  const b = hexToBase64(s);
  const pem = b.match(/.{1,64}/g).join('\n');
  return `-----BEGIN CERTIFICATE-----\n${pem}\n-----END CERTIFICATE-----`;
}
