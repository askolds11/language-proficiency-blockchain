import api from '@/apiGames';

export default {
  async getList({ page, pageSize, sort = undefined, order = undefined, search = undefined }) {
    const params = new URLSearchParams();
    params.append('page', page?.toString());
    params.append('per_page', pageSize?.toString());
    params.append('sort', sort?.toString());
    params.append('order', order?.toString());
    params.append('relatedCasesFilter ', search?.toString());
    return api().get(`/sample/games?${params.toString()}`);
  },
  async postList({ page, pageSize, sort = undefined, order = undefined, filters = undefined }) {
    const params = new URLSearchParams();
    params.append('page', (page + 1).toString());
    params.append('per_page', pageSize?.toString());
    params.append('sort', sort?.toString());
    params.append('order', order?.toString());
    return api().post(`/sample/games-secure?${params.toString()}`, {
      name: filters?.name,
      releaseDateFrom: filters?.releaseDateFrom,
      releaseDateTill: filters?.releaseDateTill,
      detailedDescription: filters?.detailedDescription,
      steamSpyOwnersFrom: Number(filters?.steamSpyOwnersFrom),
      steamSpyOwnersTill: Number(filters?.steamSpyOwnersTill),
      recommendationCountFrom: Number(filters?.recommendationCountFrom),
      recommendationCountTill: Number(filters?.recommendationCountTill),
      categoryMultiplayer: filters?.categoryMultiplayer,
      priceFrom: filters?.priceFrom !== '0.00' ? Number(filters?.priceFrom) : undefined,
      priceTill: filters?.priceTill !== '0.00' ? Number(filters?.priceTill) : undefined,
      isFree: filters?.isFree,
      metacriticScore: filters?.metacriticScore,
    });
  },
};
