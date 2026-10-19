import { SelectionModel } from "@angular/cdk/collections";
import { Location } from "@angular/common";
import { Component, OnInit, ViewChild } from "@angular/core";
import { MatListOption } from "@angular/material";
import { ActivatedRoute } from "@angular/router";
import { Grouptypeconfig } from "../../models/grouptypeconfig";
import { GrouptypeconfigService } from "../../services/grouptypeconfig.service";

@Component({
  selector: "budgetapp-config",
  templateUrl: "./config.component.html",
  styleUrls: ["./config.component.css"]
})
export class ConfigComponent implements OnInit {
  @ViewChild("miscItems")
  miscItemList;
  @ViewChild("items")
  itemList;

  configType = "";
  miscConfigType = "misc";
  groupNames = [""];

  groupOfInterest: Grouptypeconfig = {
    id: -1,
    groupName: "",
    groupType: "",
    names: [""]
  };

  miscGroup: Grouptypeconfig = {
    id: -1,
    groupName: "",
    groupType: "",
    names: [""]
  };

  selectedTransactionName = ""; // TODO: Refactor to use transactionName from groupType instead
  selectedMiscTransactionName = ""; // TODO: Refactor to use transactionName from groupType instead

  constructor(
    private groupTypeCfgService: GrouptypeconfigService,
    private route: ActivatedRoute,
    private location: Location
  ) {}

  ngOnInit() {
    this.itemList.selectedOptions = new SelectionModel<MatListOption>(false);
    this.miscItemList.selectedOptions = new SelectionModel<MatListOption>(
      false
    );
    this.groupTypeCfgService.getGroupTypeNames().subscribe(_groupNames => {
      this.groupNames = _groupNames.filter(
        groupName => groupName !== this.miscConfigType
      );
      this.configType = _groupNames[0];
      this.onRefresh(_groupNames[0]);
    });
  }

  onRefresh(groupName: string) {
    this.groupTypeCfgService.getTypeItems(groupName).subscribe(groupItems => {
      this.groupOfInterest = groupItems;
    });

    this.groupTypeCfgService
      .getTypeItems(this.miscConfigType)
      .subscribe(miscGroupItems => {
        this.miscGroup = miscGroupItems;
      });
  }

  onSelectGroupItem(groupItem: string): void {
    this.selectedTransactionName = groupItem;
    this.onRefresh(this.configType);
  }

  onSelectGroup(groupName: string): void {
    this.configType = groupName;
    this.onRefresh(groupName);
  }

  onSelectMiscGroupItem(groupItem: string): void {
    this.selectedMiscTransactionName = groupItem;
    this.onRefresh(this.configType);
  }

  onAddGroupItem(event: any): void {
    this.groupTypeCfgService
      .addTransactionName(
        this.groupOfInterest,
        this.selectedMiscTransactionName
      )
      .subscribe();

    this.onRefresh(this.configType);
  }

  onRemoveGroupItem(event: any): void {
    this.groupTypeCfgService
      .removeTransactionName(this.groupOfInterest, this.selectedTransactionName)
      .subscribe();

    this.onRefresh(this.configType);
  }

  isDataLoaded(): boolean {
    return (
      this.groupOfInterest.id !== -1 &&
      this.miscGroup.id !== -1 &&
      this.configType !== null
    );
  }
}
