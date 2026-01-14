import apiESign from '@/apiESign';

export default {
  async getMaintenanceStatus() {
    return apiESign().get('v2.0/maintenance/service');
  },
  async postMobilePrepare(JSON) {
    return apiESign().post('v2.0/mobile/prepare', JSON);
  },
  async postEIDPrepare(JSON) {
    return apiESign().post('v2.0/eid/prepare', JSON);
  },
  async postEIDSign(JSON) {
    return apiESign().post('v2.0/eid/sign', JSON);
  },
  async getFile(sessionId) {
    window.location = `${apiESign().defaults.baseURL}v2.0/file/get?sessionId=${sessionId}`;
  },
  async getFiles(JSON) {
    window.location = await apiESign().post('v2.0/eid/getMultiple', JSON);
  },
};
