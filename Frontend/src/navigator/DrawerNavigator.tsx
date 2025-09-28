import { createDrawerNavigator } from '@react-navigation/drawer';
import MapScreen from '../screens/MapScreen';
import { AllPhotosScreen } from '../screens/AllPhotosScreen';
import { AddPhotoScreen } from '../screens/AddBinPhotosScreen';

const Drawer = createDrawerNavigator();

const AppNavigator = () => {
  return (
    <Drawer.Navigator initialRouteName="Map">
      <Drawer.Screen name="Map" component={MapScreen} />
      <Drawer.Screen name="AllPhotos" component={AllPhotosScreen} />
      <Drawer.Screen name="AddPhoto" component={AddPhotoScreen} />
    </Drawer.Navigator>
  );
};
export default AppNavigator;
