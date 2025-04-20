import {GroupedStaticDataInterface} from '../interfaces';

export function getGroupedDataUtils(data: GroupedStaticDataInterface[]): [string, GroupedStaticDataInterface[]][] {
  const groupedData: { [key: string]: GroupedStaticDataInterface[] } = {};

  data.forEach((item): void => {
    if (groupedData[item.group]) {
      groupedData[item.group].push(item);
    } else {
      groupedData[item.group] = [item];
    }
  });
  // eslint-disable-next-line guard-for-in
  for (const key in groupedData) {
    groupedData[key].sort((a, b): number => a.verticalPosition < b.verticalPosition ? -1 : 1);
  }

  return Object.entries(groupedData).sort((a, b): number =>
    a[1][0].horizontalPosition < b[1][0].horizontalPosition ? -1 : 1
  );
}
