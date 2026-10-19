import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { map } from "rxjs/operators";
import { Grouptypeconfig } from "../models/grouptypeconfig";
import { UpdateTransactionName } from "./grouptypeconfig.service";

const onlyUnique = (value, index, self) => self.indexOf(value) === index;

const syncItemBuffers = (
  detailsType: string,
  _lhsItems: string[],
  _rhsItems: string[]
) => <T>(source$: Observable<Grouptypeconfig>) => {
  const lhsItems = detailsType !== "misc" ? _lhsItems : _rhsItems;
  const rhsItems = detailsType !== "misc" ? _rhsItems : _lhsItems;
  return source$.pipe(
    map(cfg => {
      return {
        id: cfg.id,
        groupName: cfg.groupName,
        groupType: cfg.groupType,
        names: cfg.names
          .concat(lhsItems)
          .filter(item => rhsItems.some(curr => curr === item) === false)
          .filter(onlyUnique)
      };
    })
  );
};

const toUpdateTransactionName = (
  cfg: Grouptypeconfig
): UpdateTransactionName => {
  return {
    id: cfg.id,
    name: cfg.groupName
  };
};

// const toUpdateTransactionName = () => (source$: Observable<Grouptypeconfig>): Observable<UpdateTransactionName> => {
//   return source$.pipe(
//     map(cfg => {
//       return {
//             id: cfg.id,
//             name: cfg.groupName
//       };
//     })
//   );
// };

@Injectable({
  providedIn: "root"
})
export class MockGrouptypeconfigService {
  serverPort = "4200";
  // GET
  // POST
  groupTypesURL = `http://localhost:${
    this.serverPort
  }/api/TransactionGroupType`;

  itemsAdded: string[] = [];
  itemsRemoved: string[] = [];

  // PUT
  // DELETE
  updateTransactionNameURL = `http://localhost:${
    this.serverPort
  }/api/TransactionGroupType/TransactionName`;

  constructor(private httpClient: HttpClient) {}

  addTransactionName(
    group: Grouptypeconfig,
    transactionName: string
  ): Observable<UpdateTransactionName> {
    this.itemsAdded = this.itemsAdded.concat([transactionName]);
    this.itemsRemoved = this.itemsRemoved.filter(
      item => item !== transactionName
    );
    return this.getTypeItems(group.groupName).pipe(
      map(toUpdateTransactionName)
    );
  }

  removeTransactionName(
    group: Grouptypeconfig,
    transactionName: string
  ): Observable<UpdateTransactionName> {
    this.itemsRemoved = this.itemsRemoved.concat([transactionName]);
    this.itemsAdded = this.itemsAdded.filter(item => item !== transactionName);
    return this.getTypeItems(group.groupName).pipe(
      map(toUpdateTransactionName)
    );
  }

  getTypeItems(detailsType: string): Observable<Grouptypeconfig> {
    const URL = `${this.groupTypesURL}.json`;

    return this.httpClient.get<Grouptypeconfig[]>(URL).pipe(
      map(cfgs => {
        return cfgs.find(cfg => {
          return cfg.groupName === detailsType;
        });
      }),
      syncItemBuffers(detailsType, this.itemsAdded, this.itemsRemoved)
    );
  }

  getGroupTypeNames(): Observable<string[]> {
    const URL = `${this.groupTypesURL}.json`;

    return this.httpClient.get<Grouptypeconfig[]>(URL).pipe(
      map(cfgs => {
        return cfgs.filter(cfg => {
          return cfg.groupType === "expense";
        });
      }),
      map(cfgs => {
        return cfgs.map(cfg => {
          return cfg.groupName;
        });
      })
    );
  }
}
