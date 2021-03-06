﻿module BitChangeSetManager.ViewModels {

    @FormViewModelDependency({
        name: "ChangeSetsViewModel",
        templateUrl: `view/changeSetsView.html`
    })
    export class ChangeSetsViewModel extends SecureViewModel {

        public changeSetsDataSource: kendo.data.DataSource;
        public deliveriesDataSource: kendo.data.DataSource;
        public customersDataSource: kendo.data.DataSource;
        public changeSetSeveritiesDataSource: kendo.data.DataSource;
        public changeSetDeliveryRequirementsDataSource: kendo.data.DataSource;

        public changeSetMetadata = BitChangeSetManagerModel.ChangeSetDto;
        public deliveryMetadata = BitChangeSetManagerModel.DeliveryDto;

        public constructor( @Inject("EntityContextProvider") public entityContextProvider: IEntityContextProvider) {
            super();
        }

        @Command()
        public async $onInit(): Promise<void> {

            let context = await this.entityContextProvider.getContext<BitChangeSetManagerContext>("BitChangeSetManager");

            let [changeSetDeliveryRequirements, changeSetSeverities] = await context.batchExecuteQuery([context.changeSetDeliveryRequirements, context.changeSetSeverities]);
            this.changeSetSeveritiesDataSource = changeSetSeverities.toQueryable(BitChangeSetManagerModel.ChangeSetSeverityDto).asKendoDataSource();
            this.changeSetDeliveryRequirementsDataSource = changeSetDeliveryRequirements.toQueryable(BitChangeSetManagerModel.ChangeSetDeliveryRequirementDto).asKendoDataSource();

            this.changeSetsDataSource = context.changeSets.asKendoDataSource({ serverPaging: true, pageSize: 5 });

            this.deliveriesDataSource = context
                .deliveries
                .map(d => { return { Id: d.Id, CustomerName: d.CustomerName, ChangeSetId: d.ChangeSetId, DeliveredOn: d.DeliveredOn } })
                .asKendoDataSource({ serverPaging: true, pageSize: 5 });

            this.deliveriesDataSource.asChildOf(this.changeSetsDataSource, ["ChangeSetId"], ["Id"]);

            this.customersDataSource = context
                .customers
                .map(c => { return { Id: c.Id, Name: c.Name } })
                .asKendoDataSource();
        }
    }
}