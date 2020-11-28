import { OSType } from './os.type';
import { PluginType } from './plugin,type';

export class Job {

  id: string;

  name: string;

  description: string;

  osType: OSType;

  pluginType: PluginType;

  isActive: boolean;

  isRunning: boolean;

  pluginPath: string;
}
