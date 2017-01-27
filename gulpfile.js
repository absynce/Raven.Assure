const _ = require('lodash/fp');
const args = require('yargs').argv;
const fs = require('fs');
const gulp = require('gulp');
const gulpDebugger = require('gulp-debug');
const gutil = require('gulp-util');
const merge = require('merge-stream');
const path = require('path');
const proc = require('child_process');
const rename = require('gulp-rename');
const usage = require('gulp-help-doc');

var paths = {
  bin: {
    prefix: 'src/Raven.Assure/bin',
    environment: 'Release',
    runtimes: [
      'win7-x64'
    ],
    target: 'net46',
    files: '**/*'
  },
  package: 'package'
};

var getPackageJson = () => JSON.parse(fs.readFileSync('./package.json', 'utf8'));

gulp.task('help', () => usage(gulp));

/**
 * Build/compile Raven.Assure.
 * Same as `dotnet build ./src/Raven.Assure/`.
 *
 * @task {build}
 */
gulp.task('build', function buildRavenAssure(done) {
   var buildProcess = proc
      .spawn('dotnet', ['build'], {
         cwd: path.join('src', 'Raven.Assure')
      });

   buildProcess.stdout.on('data', logDatar);
   buildProcess.stderr.on('data', logError);
   buildProcess.on('close', done);
});

/**
 * This is an annoying task because dotnet project.json doesn't support
 * [cross-project content copying](https://github.com/dotnet/cli/issues/753).
 */
gulp.task('copyConfigs', ['build'], function copyConfigs() {
   var configPath = path.join('src', 'Raven.Assure', 'bin', 'Debug', 'net46', 'win7-x64', 'config');
   var configs =  '{default,test.qa}.json';

   return gulp.src(path.join(configPath, configs))
      .pipe(gulpDebugger({ title: '  from:' }))
      .pipe(gulp.dest(path.join('test', 'Raven.Assure.Test', 'bin', 'Debug', 'net46', 'win7-x64', 'config')))
      .pipe(gulpDebugger({ title: '  into:' }))
      .on('error', logError);
});

/**
 * Run Raven.Assure.Test xUnit tests.
 * Same as `dotnet test ./test/Raven.Assure.Test/`.
 *
 * @task {test}
 */
gulp.task('test', ['copyConfigs'], function testRavenAssure(done) {
   var testProcess = proc
      .spawn('dotnet', ['test'], {
         cwd: path.join('test', 'Raven.Assure.Test')
      });

   testProcess.stdout.on('data', logDatar);
   testProcess.stderr.on('data', logError);
   testProcess.on('close', done);
});

/**
 * [WIP] Not working! Bump the version.
 * @task {bump}
 */
gulp.task('bump', function bumpVersion(done) {
   debugger;
   try {
      var bumpProcess = proc.spawn('bump', ['--prompt'], {
         cwd: path.join('node_modules', 'version-bump-prompt', 'bin')
      });
   }
   catch (error) {
      console.log('error', error);
   }

   bumpProcess.stdout.on('data', logDatar);
   bumpProcess.stderr.on('data', logError);
   bumpProcess.on('error', logError);
   bumpProcess.on('close', done);
});

/**
 * Package Raven.Assure CLI into ./package/{target}/assure-{version}-{environment}.
 * @task {package}
 * @arg {environment} The build environment: {Debug|Release(default)}
 * @arg {env} Alias for environment
 *
 * TODO: add branch to package output folder name, like with mobile.
 */
gulp.task('package', ['copyRavenAssureExeToAssure']);

const assurePackage = getPackageJson();
var env = args.environment || args.env || paths.bin.environment;
var getRuntimeBinPath = (runtime) => path.join(paths.bin.prefix, env, paths.bin.target, runtime, paths.bin.files);
var getRuntimePackagePath = (runtime) => path.join(paths.package, paths.bin.target, `assure-${assurePackage.version}-${runtime}-${env}`);

gulp.task('moveFromBinToPackage', ['build'], function moveFromBinToPackage(done) {
  const moveStreams = paths.bin.runtimes.map(moveFromRuntimeBinToPackage);

  return merge(moveStreams); // Returning a list of gulp streams will ensure task finishes.

  function moveFromRuntimeBinToPackage(runtime) {
    const fullRuntimeBinPath = getRuntimeBinPath(runtime);
    const fullRuntimePackagePath = getRuntimePackagePath(runtime);

    gutil.log('Packaging from:', gutil.colors.cyan(fullRuntimeBinPath));
    gutil.log('Packaging into:', gutil.colors.cyan(fullRuntimePackagePath));

    return gulp
      .src(fullRuntimeBinPath)
      .pipe(gulp.dest(fullRuntimePackagePath));
  }
});

gulp.task('copyRavenAssureExeToAssure', ['moveFromBinToPackage'], function copyRavenAssureExeToAssure() {
  const copyStreams = paths.bin.runtimes.map(copyRavenAssureExe);

  return merge(copyStreams); // Returning a list of gulp streams will ensure task finishes.

  function copyRavenAssureExe(runtime) {
    var fullRuntimePackagePath = getRuntimePackagePath(runtime);
    var copyFrom = path.join(fullRuntimePackagePath, 'Raven.Assure.exe');
    var copyToDirectory = fullRuntimePackagePath;
    var copyToFileName = 'assure.exe';

    gutil.log('Copying from:', gutil.colors.cyan(copyFrom));
    gutil.log('Copying into:', gutil.colors.cyan(path.join(copyToDirectory, copyToFileName)));

    return gulp
      .src(copyFrom)
      .pipe(rename(copyToFileName))
      .pipe(gulp.dest(copyToDirectory));
   }
});

// Define reusable loggers.
var toString = (msg) => msg.toString();
// dotnet CLI adds line break, so when combined with log it adds an additional empty line.
// This replaces that line break with an empty string.
var removeLineBreaks = (msg) => msg.replace(/(\r\n|\r|\n)/g, '');
var logDatar = _.flow(toString, removeLineBreaks, gutil.log);
var logError = _.flow(gutil.colors.red, logDatar);
