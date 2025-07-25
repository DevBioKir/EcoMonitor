import { createDrawerNavigator } from "@react-navigation/drawer";
import { NavigationContainer } from "@react-navigation/native";
import { RootNavigator } from "./RootNavigator";

const Drawer = createDrawerNavigator();

const AppNavigator = () => {
    return(
        <NavigationContainer>
            <Drawer.Navigator initialRouteName="Main">
                <Drawer.Screen name="Main" component={RootNavigator} />
            </Drawer.Navigator>
        </NavigationContainer>
    );
};
export default AppNavigator;