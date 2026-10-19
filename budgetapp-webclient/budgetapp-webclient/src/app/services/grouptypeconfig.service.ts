import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { map } from "rxjs/operators";
import { Grouptypeconfig } from "../models/grouptypeconfig";

export class UpdateTransactionName {
  id: number;
  name: string;
}

@Injectable()
export class GrouptypeconfigService {
  serverPort = "5000";
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
    const cudOptions = {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        Accept: "application/json"
      })
    };

    const URL = `${this.updateTransactionNameURL}`;
    return this.httpClient.put<UpdateTransactionName>(
      URL,
      { id: group.id, name: transactionName },
      cudOptions
    );
  }

  removeTransactionName(
    group: Grouptypeconfig,
    transactionName: string
  ): Observable<UpdateTransactionName> {
    const cudOptions = {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        Accept: "application/json"
      }),
      body: { id: `${group.id}`, name: `${transactionName}` }
    };

    const URL = `${this.updateTransactionNameURL}`;
    return this.httpClient.delete<UpdateTransactionName>(URL, cudOptions);
  }

  getTypeItems(detailsType: string): Observable<Grouptypeconfig> {
    const URL = `${this.groupTypesURL}`;

    return this.httpClient.get<Grouptypeconfig[]>(URL).pipe(
      map(cfgs => {
        return cfgs.find(cfg => {
          return cfg.groupName === detailsType;
        });
      })
    );
  }

  getGroupTypeNames(): Observable<string[]> {
    const URL = `${this.groupTypesURL}`;

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
